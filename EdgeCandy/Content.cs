using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EdgeCandy
{
    /// <summary>
    /// One-stop shop for all of your content needs.
    /// Since the game is small, everything is loaded at the beginning,
    /// so just grab what you need when you need it.
    /// </summary>
    public static class Content
    {
        // These properties are ordered by type, then by name

        #region Fonts
        public static Font Font { get; set; }
        #endregion

        #region Textures
        public static Texture TestSplash { get; set; }
        #endregion

        #region Music
        #endregion

        #region SoundBuffers
        #endregion

        #region LevelFragments
        // TODO: implement LevelFragment
        #endregion

        /// <summary>
        /// Loads all game content for later use.
        /// Should only be called during initialization.
        /// </summary>
        public static void Load()
        {
            Font = new Font("Content/font.ttf");

            TestSplash = new Texture("Content/test.png");
        }
    }
}
