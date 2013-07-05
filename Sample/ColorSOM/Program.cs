//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace ColorSOM
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using SelfOrganizingMap;

    /// <summary>
    /// A demonstration of using a self-organizing map where the input data is a randomized color pallet.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point into the demonstration.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Justification = "It is safe to suppress a warning from this rule if the code library will not be localized.")]
        public static void Main()
        {
            Console.WriteLine("Organizing an image of colors using a self-organizing-map. This may take a few seconds.");

            IList<Vector> trainingData = GetTrainingData();

            Map map = new Map(width: 150, height: 150, depth: 3);
            map.Train(trainingData);

            SaveImageFromMap(map);

            Console.WriteLine("Hit any key to continue.");
            Console.Read();
        }

        /// <summary>
        /// Gets a list of color data with which to train a self-organizing map.
        /// </summary>
        /// <returns>Returns a list of color data with which to train a self-organizing map</returns>
        private static IList<Vector> GetTrainingData()
        {
            IList<Vector> trainingData = new List<Vector>();
            
            Random random = new Random();

            for (int i = 0; i < 30; i++)
            {
                int red = random.Next(256);
                int green = random.Next(256);
                int blue = random.Next(256);

                trainingData.Add(new Vector { red, green, blue });
            }

            return trainingData;
        }

        /// <summary>
        /// Saves the an image using the given trained map.
        /// </summary>
        /// <param name="map">The map that has been trained with color data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object)", Justification = "It is safe to suppress a warning from this rule if the code library will not be localized.")]
        private static void SaveImageFromMap(Map map)
        {
            using (Bitmap image = new Bitmap(map.Width, map.Height))
            {
                for (int x = 0; x < image.Height; ++x)
                {
                    for (int y = 0; y < image.Width; ++y)
                    {
                        MapNode mapNode = map[x, y];
                        image.SetPixel(x, y, Color.FromArgb((int)mapNode.Weights[0], (int)mapNode.Weights[1], (int)mapNode.Weights[2]));
                    }
                }

                string path = Path.Combine(Environment.CurrentDirectory, "image.png");
                image.Save(path, ImageFormat.Png);

                Console.WriteLine("Image created at {0}.", path);
            }
        }
    }
}