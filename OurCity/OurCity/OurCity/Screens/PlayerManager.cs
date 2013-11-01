using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OurCity.Screens
{
    /// <summary>
    /// Singleton for managing players stuff
    /// </summary>
    class PlayerManager
    {
        bool initialized;
        public GameplayScreen GameScreen { get; internal set; }

        #region Constructor
        private static PlayerManager instance;

        public static PlayerManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerManager();

                return instance;
            }
        }

        private PlayerManager()
        {
            initialized = false;
        }
        #endregion

        public void Initialize(GameplayScreen gameScreen) {
            GameScreen = gameScreen;
            initialized = true;
        }
    }
}
