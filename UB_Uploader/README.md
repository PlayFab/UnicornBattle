
##Back-end Setup:
We built [this handy tool](/UB_Uploader/ "UB_Uploader") to clone the Unicorn Battle back-end to a PlayFab title of your choosing. 

##Creating your Unicorn Battle clone:
  1. Download the directory or compile the solution
  2. Update the *(/PlayFabData/TitleSettings.json)[/UB_Uploader/UploaderAndData/PlayFabData/TitleSettings.json]* file with your **TitleId** and corresponding **DeveloperSecretKey**. 
  3. Locate and run *(UB_Uploader.exe)[/UB_Uploader/UploaderAndData/UB_Uploader.exe]*
  4. A log *([PreviousUploadLog.txt](https://github.com/PlayFab/UnicornBattle/blob/master/UB_Uploader/UploaderAndData/PreviousUploadLog.txt))* of the results will be generated within the exe's working directory. 
 
If no errors were reported, then you are ready to move on to the client setup!

![UB_Uploader](https://github.com/PlayFab/UnicornBattle/wiki/Assets/Images/UB_Uploader.png "UB_Uploader")

See the [Back-end Guide](https://github.com/PlayFab/UnicornBattle/wiki/1-Unicorn-Battle-Backend-Guide) for additional details.

