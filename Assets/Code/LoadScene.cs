using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string LoadSceneName;
    public string OpenURL;

#if UNITY_SWITCH
    public nn.account.UserHandle userHandle;
    public nn.account.Uid userId;

    private string mountName = "DadLeftMeSave";
    private const string fileName = "DadLeftMeSaveData";
    private string filePath;

    private const int datasize = 128;
#endif

    private void Start()
    {
#if UNITY_SWITCH
        nn.account.Account.Initialize();
        userHandle = new nn.account.UserHandle();
        nn.account.Account.TryOpenPreselectedUser(ref userHandle);
        nn.account.Account.GetUserId(ref userId, userHandle);

        nn.Result result = nn.fs.SaveData.Mount(mountName, userId);
        result.abortUnlessSuccess();

        nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();
        filePath = string.Format("{0}:/{1}", mountName, fileName);

        nn.fs.EntryType entryType = 0;
        result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
        if (nn.fs.FileSystem.ResultPathNotFound.Includes(result)) { return; }
        result.abortUnlessSuccess();



        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);

        if (!result.IsSuccess())
        {
            result = nn.fs.File.Create(filePath, datasize);
            result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        }
        nn.fs.File.Close(fileHandle);
#endif
    }

    void Update()
    {
        SceneManager.LoadScene(LoadSceneName);
        if (OpenURL.Length > 1)
        {
            Application.OpenURL(OpenURL);

        }
    }
}
