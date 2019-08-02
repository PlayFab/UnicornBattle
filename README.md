# Introducing Unicorn Battle
This repository is the home of Unicorn Battle (UB), PlayFab's open-source, end-to-end demo project. UB is a complete game sporting dynamic gameplay, driven by PlayFab's back-end technology, showcasing how to build modern live-game systems. 

![Splash Screen](https://github.com/PlayFab/UnicornBattle/wiki/Assets/Images/SplashScreen_01.png "Splash Screen")

## Key Features
  * **Unified Player Profile** - Accounts uses Device ID &/or Facebook Authentication
  * **Cloud Saving** - Store data on up to 6 characters per account
  * **Inventory Management** - Transfer items and currency between the player and its characters
  * **Dynamic Gameplay** - Drive client behaviors using TitleData
  * **Virtual Items, Currencies, and Stores** - Configured via PlayFab's Game Manager
  * **Friend Lists** - Import Facebook friends or add your own
  * **Statistics Tracking** - Used to drive Achievements and real-time leaderboards
  * **Push Notifications** - for re-engagement campaigns and special offers
  * **Real-money, In-app Purchases** - with Android or iOS receipt validation
  * **CDN-hosted AssetBundles** - Dynamically stream content to clients for new sales and events
  * **Cloud-hosted Logic** - Cloud Script methods that call a variety of  protected APIs
  * **Community Forums** - with API access to your game; built by our partners over at [Innervate](https://playfab.com/marketplace/innervate/"A turn-key community solution that comes out-of-the-box integrated with PlayFab")

![Battle](https://github.com/PlayFab/UnicornBattle/wiki/Assets/Images/Battle2.png "Battle")

## Configuration and Setup
### Prerequisites:
This is an advanced example and can be quite dense at times due to the many sub-systems. 

- You should be very familiar with Unity3d & UUI 
- Have a [basic understanding](https://api.playfab.com/) of the PlayFab API
- Be comfortable working with JSON-to-C# serialization & deserialization

### Back-end Setup:
The first step is cloning the Unicorn Battle back-end to your own PlayFab title so you can play around with all the data on your own.

We've built a [handy tool](https://github.com/PlayFab/UnicornBattle/tree/master/UB_Uploader "https://github.com/PlayFab/UnicornBattle/tree/master/UB_Uploader") to do this -- see the [Back-end guide](https://github.com/PlayFab/UnicornBattle/wiki/1-Unicorn-Battle-Backend-Guide) for full instructions.

### Client Setup:
The next step is to get the Unicorn Battle client running on your own mobile device, and communicating with your own back-end.

There are two ways to setup the Unicorn Battle client. You can download the Unity source code, compile it yourself, and run on the device of your choice. Or, you can download and install our precompiled Unicorn Battle app directly from the Google Play store for Android.

See the [Client Guide](https://github.com/PlayFab/UnicornBattle/wiki/2-Unicorn-Battle-Client-Guide) for full instructions.

## More information:
For a complete list of available APIs, check out the [online documentation](http://api.playfab.com/).

Our Developer Success Team can assist with answering any questions as well as process any feedback you have about PlayFab services.
[Community, Forums, Support and Knowledge Base](https://support.playfab.com/support/home)

## Copyright and Licensing Information:

  Apache License -- 
  Version 2.0, January 2004
  http://www.apache.org/licenses/

  License Details available in [LICENSE.txt](https://github.com/PlayFab/UnicornBattle/blob/master/LICENSE "Apache 2.0 License")

### Third-Party Code Attribution:
The asset creators below retain all rights to their assets. Please do not use without explicit consent. If you like any of the assets used in Unicorn Battle, support the creators:

- **Featured Art**: [Ashley Sarroca](mailto:sarroca.a@gmail.com "Creator's Email") -- [http://ashleysarroca.com/](http://ashleysarroca.com/ "Creator's Site")
- **[GUI DarkStone 1](https://www.assetstore.unity3d.com/en/#!/content/18225 "https://www.assetstore.unity3d.com/en/#!/content/18225")**: [Layer Lab](http://www.layerlab.io/talk/ "Creator's Contact") -- [http://www.layerlab.io/work](http://www.layerlab.io/work "Creator's Site")
- [**Cartoon FX Package**](https://www.assetstore.unity3d.com/en/#!/content/4010 "https://www.assetstore.unity3d.com/en/#!/content/4010"): 
[Jean Moreno](http://www.jeanmoreno.com/contact.html "Creator's Contact") -- [http://www.jeanmoreno.com/](http://www.jeanmoreno.com/ "Creator's Site")
- [**UGuiTween**](https://www.assetstore.unity3d.com/en/#!/content/26547 "https://www.assetstore.unity3d.com/en/#!/content/26547"): [Chris Cunningham](http://rightsomegoodgames.ca/contact.html "Creator's Contact") -- [http://rightsomegoodgames.ca/](http://rightsomegoodgames.ca/ "Creator's Site")
- **[Painterly Spell Icons](http://opengameart.org/content/painterly-spell-icons-part-1 "http://opengameart.org/content/painterly-spell-icons-part-1")**: [J. W. Bjerk](mailto:me@jwbjerk.com "Crator's Contact") (eleazzaar) -- find this and other open art at: [http://opengameart.org](http://opengameart.org "http://opengameart.org")
- **[Facebook SDK](https://developers.facebook.com/docs/unity/ "https://developers.facebook.com/docs/unity/")**: [Facebook](https://developers.facebook.com/ "Crator's Contact") -- [https://developers.facebook.com/](https://developers.facebook.com/ "https://developers.facebook.com/")
- **[OpenIAB](https://github.com/onepf/OpenIAB "https://github.com/onepf/OpenIAB")**: [OpenIAB](http://onepf.org/openiab/ "Crator's Contact") -- [http://onepf.org/openiab/](http://onepf.org/openiab/ "http://onepf.org/openiab/")

## Community Development
Ideas on how we can make our examples, documentation and services better? -- **[Contact Us](mailto:devrel@playfab.com "PlayFab")** 
  
Want to continue developing on Unicorn Battle? -- **[Fork Unicorn Battle](https://github.com/PlayFab/UnicornBattle#fork-destination-box "https://github.com/PlayFab/UnicornBattle#fork-destination-box")**

*We love to hear from our developer community!*

## Version History:
See [GitHub Releases](https://github.com/PlayFab/UnicornBattle/releases "GitHub Versions") for the latest stable build and patch notes.
