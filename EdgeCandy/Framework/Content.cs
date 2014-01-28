using SFML.Graphics;
using TiledSharp;

namespace EdgeCandy.Framework
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
        public static Texture Player { get; set; }
        public static Texture Ball { get; set; }
        public static Texture Tileset { get; set; }

        public static Texture Pixel { get; set; }
        #endregion

        #region Music
        #endregion

        #region SoundBuffers
        #endregion

        #region TmxMaps
        public static TmxMap TestMap { get; set; }
        #endregion

        /// <summary>
        /// Loads all game content for later use.
        /// Should only be called during initialization.
        /// </summary>
        public static void Load()
        {
            Font = new Font("Content/font.ttf");

            Player = new Texture("Content/Sprites/player.png");
            Ball = new Texture("Content/Sprites/deathsphere.png");
            Tileset = new Texture("Content/Sprites/tileset.png");

            TestMap = new TmxMap("Content/Levels/test.tmx");

            var image = new Image(1, 1, Color.White);
            Pixel = new Texture(image);
        }
    }
}
