using System;
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
        private static Random rand = new Random();

        #region Fonts
        public static Font Font { get; set; }
        #endregion

        #region Textures
        public static Texture Ball { get; set; }
        public static Texture CandyCane { get; set; }

        private static Texture[] chocolates;
        public static Texture Chocolate
        {
            get { return chocolates[rand.Next(chocolates.Length)]; }
        }

        public static Texture DoubleCandyCane { get; set; }
        public static Texture Player { get; set; }
        public static Texture Powerup { get; set; }

        private static Texture[] ranchers;
        public static Texture Rancher
        {
            get { return ranchers[rand.Next(ranchers.Length)]; }
        }
        public static Texture Tileset { get; set; }

        public static Texture Pixel { get; set; }
        #endregion

        #region Music
        #endregion

        #region SoundBuffers
        #endregion

        #region TmxMaps
        public static TmxMap TestMap { get; set; }
        public static TmxMap Level { get; set; }
        #endregion

        /// <summary>
        /// Loads all game content for later use.
        /// Should only be called during initialization.
        /// </summary>
        public static void Load()
        {
            Font = new Font("Content/font.ttf");

            Ball = new Texture("Content/Sprites/deathsphere.png");
            CandyCane = new Texture("Content/Sprites/Candy/candy_cane.png");
            chocolates = new[] 
                {
                    new Texture("Content/Sprites/Candy/dark_chocolate.png") { Repeated = true },
                    new Texture("Content/Sprites/Candy/milk_chocolate.png") { Repeated = true },
                    new Texture("Content/Sprites/Candy/white_chocolate.png") { Repeated = true }
                };

            DoubleCandyCane = new Texture("Content/Sprites/Candy/double_candy_cane.png");
            Player = new Texture("Content/Sprites/player.png");
            Powerup = new Texture("Content/Sprites/powerup.png");

            ranchers = new[]
                {
                    new Texture("Content/Sprites/Candy/rancher_green.png"),
                    new Texture("Content/Sprites/Candy/rancher_purple.png"),
                    new Texture("Content/Sprites/Candy/rancher_red.png"),
                    new Texture("Content/Sprites/Candy/rancher_teal.png")
                };
            Tileset = new Texture("Content/Sprites/tileset.png");

            TestMap = new TmxMap("Content/Levels/test.tmx");
            Level = new TmxMap("Content/Levels/level.tmx");

            var image = new Image(1, 1, Color.White);
            Pixel = new Texture(image);
        }
    }
}
