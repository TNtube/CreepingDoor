using System;
using UnityEngine;
using UnityEditor; // Ensure this is included for AssetDatabase
using System.IO;
using System.Threading.Tasks;
using Anvil.Common;

namespace Anvil.EditorEmojis
{
    internal class Symbol
    {
        internal string Name { get; private set; }
        internal string ImageName { get; private set; }
        internal string ImagePath { get; private set; }

        private Texture2D _previewTexture;
        internal Texture2D PreviewTexture
        {
            get
            {
                if (_previewTexture == null && !isTextureNull)
                {
                    LoadPreviewTexture();
                }
                return _previewTexture;
            }
            private set
            {
                _previewTexture = value;
            }
        }
        internal IconColor IconColor { get; private set; }

        internal bool isTextureNull = false;
        
        internal Symbol(string name, string imageName)
        {
            Name = name;
            ImageName = imageName;
        }

        /// <summary>
        /// Initializes the symbol by setting the image path.
        /// Texture loading is deferred until the Texture property is accessed.
        /// </summary>
        /// <param name="symbolImageDir">Directory where symbol images are stored.</param>
        /// <param name="color">Optional color to apply to the texture.</param>
        internal void Initialize(string symbolImageDir, IconColor iconColor)
        {
            ImagePath = Path.Combine(symbolImageDir, $"{ImageName}.png");
            IconColor = iconColor;
        }
        /// <summary>
        /// Loads the texture from the AssetDatabase or from file if necessary.
        /// </summary>
        private void LoadPreviewTexture()
        {
            if (_previewTexture != null || isTextureNull)
                return;

            // Attempt to load the texture from the AssetDatabase
            _previewTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ImagePath);

            if (_previewTexture == null)
            {
                isTextureNull = true;
                Debug.LogWarning($"No Image found for {Name} at {ImagePath}");
                return;
            }

            // If a color is specified, apply it to the texture
            if (IconColor != IconColor.None && IconColor != IconColor.White)
            {
                _previewTexture = ColorTexture(ImagePath, Constants.IconColors[IconColor]);
            }
        }
        /// <summary>
        /// Applies the specified color to the texture.
        /// </summary>
        /// <param name="path">Path to the texture file.</param>
        /// <param name="color">Color to apply.</param>
        /// <returns>Colored Texture2D object.</returns>
        private Texture2D ColorTexture(string path, Color color)
        {
            // Load the texture from file using ReadAllBytes
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D originalTexture = new(1, 1, TextureFormat.RGBA32, false); // Ensure texture format supports alpha
            if (!originalTexture.LoadImage(fileData))
            {
                Debug.LogError($"Failed to load texture from path: {path}");
                return null;
            }

            // Get the original pixels
            Color[] pixels = originalTexture.GetPixels();

            // Parallelize the pixel manipulation
            Parallel.For(0, pixels.Length, i =>
            {
                // Retain original alpha, apply new RGB
                pixels[i] = new Color(color.r, color.g, color.b, pixels[i].a);
            });

            // Create a new texture for the colored result
            Texture2D coloredTexture = new(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);

            // Set the colored pixels on the new texture
            coloredTexture.SetPixels(pixels);
            coloredTexture.Apply();

            return coloredTexture;
        }

        /// <summary>
        /// Changes the color of the texture. This will reload and recolor the texture.
        /// </summary>
        /// <param name="color">New color to apply.</param>
        internal void ChangeColor(IconColor iconColor)
        {
            IconColor = iconColor;
            // Reset the texture so it reloads with the new color
            _previewTexture = null;
            isTextureNull = false;
        }

        /// <summary>
        /// Retrieves the serialized texture. If a colored version exists in the AssetDatabase,
        /// it loads and returns it. Otherwise, it creates a new colored asset from the current
        /// PreviewTexture and returns the newly created texture.
        /// </summary>
        /// <returns>The serialized Texture2D object.</returns>
        internal Texture2D GetSerializedTexture()
        {
            // If no color is applied, return the original preview texture, which is loaded from the asset database.
            if (IconColor == IconColor.None || IconColor == IconColor.White)
            {
                Util.Log("GetSerializedTexture: No color applied, returning original preview texture.", LogFilter.Model);
                return PreviewTexture;
            }

            string colorSuffix = $"{IconColor.ToString().ToLower()}";
            string basePathWithoutExtension = Path.GetDirectoryName(ImagePath);
            string fileName = Path.GetFileNameWithoutExtension(ImagePath);
            string newAssetPath = Path.Combine(basePathWithoutExtension, "Cached", $"{fileName}_{colorSuffix}.png");

            // Ensure the asset path uses forward slashes
            newAssetPath = newAssetPath.Replace("\\", "/");

            // Asset paths in Unity should be relative to the 'Assets' folder
            if (!newAssetPath.StartsWith("Assets/"))
            {
                newAssetPath = "Assets/" + newAssetPath;
            }

            // Attempt to load the colored texture from AssetDatabase
            Texture2D coloredTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(newAssetPath);
            if (coloredTexture != null)
            {
                Util.Log($"GetSerializedTexture: Found colored texture in AssetDatabase: {newAssetPath}", LogFilter.Model);
                return coloredTexture;
            }

            // If the colored texture doesn't exist, create it from PreviewTexture
            if (PreviewTexture != null)
            {
                Util.Log($"GetSerializedTexture: Creating new colored texture: {newAssetPath}", LogFilter.Model);

                // Encode the texture to PNG
                byte[] pngData = PreviewTexture.EncodeToPNG();

                // Ensure the directory exists
                string directory = Path.GetDirectoryName(newAssetPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write the PNG data to the asset path
                File.WriteAllBytes(newAssetPath, pngData);

                // Import the asset
                AssetDatabase.ImportAsset(newAssetPath);

                // Optionally, set the texture importer settings
                TextureImporter importer = AssetImporter.GetAtPath(newAssetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.GUI;
                    importer.alphaIsTransparency = true;
                    importer.isReadable = true;
                    AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceUpdate);
                }

                // Load and return the newly created asset
                coloredTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(newAssetPath);
                return coloredTexture;
            }

            Debug.LogError($"PreviewTexture is null for symbol {Name}. Cannot serialize texture.");
            return null;
        }

    }
}
