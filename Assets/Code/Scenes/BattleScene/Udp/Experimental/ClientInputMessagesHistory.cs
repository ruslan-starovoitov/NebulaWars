using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    /// <summary>
    /// Хранит последние n вводов игроков.
    /// Ввод отправляется несколько раз для того, чтобы сервер получил все вводы.
    /// </summary>
    public class ClientInputMessagesHistory
    {
        private readonly int matchId;
        private readonly ushort playerTmpId;
        private readonly SelfCleaningDictionary<InputMessageModel> dict;
        private readonly InputModelValidator inputModelValidator = new InputModelValidator();
        private readonly InputMessageIdFactory inputMessageIdFactory = new InputMessageIdFactory();

        public ClientInputMessagesHistory(ushort playerTmpId, int matchId)
        {
            this.playerTmpId = playerTmpId;
            this.matchId = matchId;
            dict = new SelfCleaningDictionary<InputMessageModel>(10);
        }
        
        public void AddInput(InputMessageModel model)
        {
            inputModelValidator.Validate(model);
            
            int inputId = inputMessageIdFactory.Create();
            dict.Add(inputId, model);
        }

        public InputMessagesPack GetInputModelsPack()
        {
            Dictionary<int, InputMessageModel> history = dict.Read();
            if (history.Count > 10)
            {
                throw new Exception("Коллекция работает неправильно");
            }
            
            InputMessagesPack pack = new InputMessagesPack()
            {
                MatchId = matchId,
                TemporaryId = playerTmpId,
                History = history
            };
            return pack;
        }

        public List<InputMessageModel> Get(int tickNumber)
        {
            Dictionary<int, InputMessageModel> allHistory = dict.Read();
            return allHistory.Values.Where(model => model.TickNumber == tickNumber).ToList();
        }

        public InputMessageModel GetLast()
        {
            return dict.GetLast();
        }
    }
}