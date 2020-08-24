using System.Collections.Generic;
using System.Linq;
using Code.Scenes.LobbyScene.ECS.Warships.Utils;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.EntitasCore.Entitas.Components;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.AccountData.AccountDataChangingHandlers
{
    /// <summary>
    /// Отвечает за обновление всех данных аккаунта.
    /// </summary>
    public class AccountDtoComponentsCreatorSystem:IExecuteSystem
    {
        private bool hasNewValue;
        private AccountDto accountData;
        private readonly LobbyUiContext lobbyUiContext;
        private readonly ILog log = LogManager.CreateLogger(typeof(AccountDtoComponentsCreatorSystem));

        public AccountDtoComponentsCreatorSystem(Contexts contexts)
        {
            lobbyUiContext = contexts.lobbyUi;
        }
        
        /// <summary>
        /// Вызывается при получении данных от профиль-сервера.
        /// </summary>
        /// <param name="accountInfoArg"></param>
        public void SetAccountDto(AccountDto accountInfoArg)
        {
            log.Info("Пришли данные аккаунта");
            accountInfoArg.CheckAccountData(log);
            accountData = accountInfoArg;
            hasNewValue = true;
        }
        
        public int GetCurrentWarshipId()
        {
            WarshipTypeEnum warshipTypeEnum = lobbyUiContext.currentWarshipTypeEnum.value;
            WarshipComponent currentWarshipComponent = lobbyUiContext
                .GetGroup(LobbyUiMatcher.Warship)
                .AsEnumerable()
                .Single(entity => entity.warship.warshipDto.WarshipTypeEnum == warshipTypeEnum).warship;
            int id = currentWarshipComponent.warshipDto.Id;
            return id;
        }

        public void Execute()
        {
            if (hasNewValue)
            {
                lobbyUiContext.ReplaceUsername(accountData.Username);
                lobbyUiContext.ReplaceAccountRating(accountData.AccountRating);
                lobbyUiContext.ReplaceHardCurrency(accountData.HardCurrency);
                lobbyUiContext.ReplaceSoftCurrency( accountData.SoftCurrency);
                WarshipTypeEnum warshipTypeEnum = CurrentWarshipTypeStorage.ReadWarshipType();
                lobbyUiContext.ReplaceCurrentWarshipTypeEnum(warshipTypeEnum);
                lobbyUiContext.ReplacePointsForSmallLootbox(accountData.SmallLootboxPoints);
                CreateWarshipComponents(accountData.Warships, lobbyUiContext);
                hasNewValue = false;
            }
        }

        private void CreateWarshipComponents(List<WarshipDto> warships, LobbyUiContext lobbyUiContextArg)
        {
            log.Info("Старт создания сущностей для кораблей");
            for (var i = 0; i < warships.Count; i++)
            {
                log.Info("Создание сущности для корабля " + i);
                WarshipDto warshipDto = warships[i];
                lobbyUiContext.CreateEntity().AddWarship(warshipDto);
            }
            
            log.Info("Создание сущностей для кораблей окончено");
        }
    }
}