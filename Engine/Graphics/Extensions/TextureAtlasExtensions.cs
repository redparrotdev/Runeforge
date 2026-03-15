using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Engine.Graphics.Extensions;

public static class TextureAtlasExtensions
{
    public static SpriteAtlas LoadTextureAtlasFromXml(this ContentManager content, string file)
    {
        var fullPath = Path.Combine(content.RootDirectory, file);

        using var fs = File.OpenRead(fullPath);
        using var reader = XmlReader.Create(fs);

        XDocument doc = XDocument.Load(reader);
        var docRoot = doc.Root;

        var texturePath = docRoot.Element("Texture").Value;
        var texture = content.Load<Texture2D>(texturePath);

        var atlas = new SpriteAtlas(texture);

        var regions = (docRoot.Element("Sprites")?.Elements("Sprite") ?? []).ToArray();
        
        foreach (var node in regions)
        {
            var name = node.Attribute("name")?.Value;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var x = int.Parse(node.Attribute("x")?.Value ?? "0");
            var y = int.Parse(node.Attribute("y")?.Value ?? "0");
            var w = int.Parse(node.Attribute("width")?.Value ?? "0");
            var h = int.Parse(node.Attribute("height")?.Value ?? "0");
            var originX = float.Parse(node.Attribute("originX")?.Value ?? $"{w / 2f}");
            var originY = float.Parse(node.Attribute("originY")?.Value ?? $"{h / 2f}");

            var sourceRect = new Rectangle(x, y, w, h);
            var origin = new Vector2(originX, originY);
           
            atlas.AddSprite(name, new Sprite(texture, sourceRect, origin));
        }

        var animations = (docRoot.Element("Animations")?.Elements("Animation") ?? []).ToArray();
        foreach (var node in animations)
        {
            var name = node.Attribute("name")?.Value;
            var frames = (node.Elements("Frame") ?? []).ToArray();

            if (string.IsNullOrWhiteSpace(name) || frames.Length == 0) continue;

            var frameRate = float.Parse(node.Attribute("fps").Value ?? "0");
            var animationFrames = new List<Sprite>(frames.Length);
            foreach (var frame in frames)
            {
                var regionName = frame.Attribute("sprite")?.Value;
                if (string.IsNullOrWhiteSpace(regionName)) continue;

                var region = atlas.GetSprite(regionName);
                animationFrames.Add(region);
            }

            if (animationFrames.Count == 0) continue;

            var animation = new Animation(animationFrames, frameRate);
            atlas.AddAnimation(name, animation);
        }

        return atlas;
    }
}
