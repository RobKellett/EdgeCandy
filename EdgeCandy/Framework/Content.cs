using System;
using SFML.Audio;
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
        public static Random Random = new Random();

        #region Fonts
        public static Font Font { get; set; }
        #endregion

        #region Textures
        public static Texture CandyCane { get; set; }

        private static Texture[] chocolates;
        public static Texture Chocolate
        {
            get { return chocolates[Random.Next(chocolates.Length)]; }
        }

        public static Texture DoubleCandyCane { get; set; }
        public static Texture Logo { get; set; }
        public static Texture MeterBack { get; set; }
        public static Texture MeterFront { get; set; }
        public static Texture Noise { get; set; }
        public static Texture Player { get; set; }
        public static Texture Powerup { get; set; }

        private static Texture[] ranchers;
        public static Texture Rancher
        {
            get { return ranchers[Random.Next(ranchers.Length)]; }
        }
        public static Texture Tileset { get; set; }

        public static Texture Pixel { get; set; }
        #endregion

        #region Music
        public static Music Music { get; set; }
        #endregion

        #region SoundBuffers
        public static SoundBuffer Jump { get; set; }
        public static SoundBuffer NoiseSound { get; set; }
        public static SoundBuffer Hit { get; set; }
        public static SoundBuffer PowerupSound { get; set; }
        public static SoundBuffer Shatter { get; set; }
        public static SoundBuffer Slice { get; set; }
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

            CandyCane = new Texture("Content/Sprites/Candy/candy_cane.png");
            chocolates = new[] 
                {
                    new Texture("Content/Sprites/Candy/dark_chocolate.png") { Repeated = true },
                    new Texture("Content/Sprites/Candy/milk_chocolate.png") { Repeated = true },
                    new Texture("Content/Sprites/Candy/white_chocolate.png") { Repeated = true }
                };

            DoubleCandyCane = new Texture("Content/Sprites/Candy/double_candy_cane.png");
            Logo = new Texture("Content/SPrites/logo.png");
            MeterBack = new Texture("Content/Sprites/meter_back.png");
            MeterFront = new Texture("Content/Sprites/meter_front.png");
            Noise = new Texture("Content/Sprites/noise.png");
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

            Music = new Music("Content/Music/CandyRush.ogg") { Loop = true };

            Jump = new SoundBuffer("Content/Sounds/jump.wav");
            NoiseSound = new SoundBuffer("Content/Sounds/noise.wav");
            Hit = new SoundBuffer("Content/Sounds/hit.wav");
            PowerupSound = new SoundBuffer("Content/Sounds/powerup.wav");
            Shatter = new SoundBuffer("Content/Sounds/shatter.wav");
            Slice = new SoundBuffer("Content/Sounds/slice.wav");

            TestMap = new TmxMap("Content/Levels/test.tmx");
            Level = new TmxMap("Content/Levels/level.tmx");

            var image = new Image(1, 1, Color.White);
            Pixel = new Texture(image);
        }
    }
}
