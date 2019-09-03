using UnityEngine;

namespace UnicornBattle.Controllers
{

    public class CanvasController : Singleton<CanvasController>
        {
            protected CanvasController() { } // guarantee this will be always a singleton only - can't use the constructor!

            public Transform settingsMenu;
            public Transform modalAlert;
        }

}