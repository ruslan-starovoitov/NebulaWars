using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Http;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.Scripts
{
    public class UsernameMenuController : MonoBehaviour
    {
        private AuthSingleton authSingleton;
        [SerializeField] private Text errorText;
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private GameObject errorRoot;
        [SerializeField] private InputField usernameInput;
        private readonly ILog log = LogManager.CreateLogger(typeof(UsernameMenuController));
        
        private void Start()
        {
            authSingleton = AuthSingleton.Instance();
        }

        public void ShowMenu()
        {
            menuRoot.SetActive(true);
            PlayerIdStorage.TryGetUsername(out var username);
            usernameInput.text = username;
        }

        public void ConfirmChanges()
        {
            StartCoroutine(ChangeUsername());
        }
        
        private IEnumerator ChangeUsername()
        {
            log.Info($"Вызов смены ника. Новый:{usernameInput.text}");
            var task = authSingleton.TrySetUsernameAsync(usernameInput.text);
            yield return new WaitUntil(()=>task.IsCompleted);
            if (task.Status != TaskStatus.RanToCompletion)
            {
                log.Error("Не удалось обновить ник. "+task.Status);
                ShowError("Unexpected error.");
            }
            else
            {
                switch (task.Result)
                {
                    case UsernameValidationResultEnum.Ok:
                        log.Info($"Ник успешно обновлён. Новый:{usernameInput.text}");
                        menuRoot.SetActive(false);
                        break;
                    case UsernameValidationResultEnum.TooLong:
                        ShowError("The username is too long.");
                        break;
                    case UsernameValidationResultEnum.TooShort:
                        ShowError("The username is too short.");
                        break;
                    case UsernameValidationResultEnum.InvalidCharacter:
                        ShowError("Invalid character.");
                        break;
                    case UsernameValidationResultEnum.ContainsSpace:
                        ShowError("Spaces are not allowed.");
                        break;
                    case UsernameValidationResultEnum.DoesNotBeginWithALetter:
                        ShowError("Username must start with a letter");
                        break;
                    case UsernameValidationResultEnum.OtherError:
                        ShowError("Unexpected error.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ShowError(string errorMessage)
        {
            errorText.text = $"<color='red'>{errorMessage}</color>\n<i>Click to return.</i>";
            errorRoot.SetActive(true);
            UiSoundsManager.Instance().PlayError();
        }
    }
}