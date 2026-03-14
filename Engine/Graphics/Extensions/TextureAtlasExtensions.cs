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
    public static TextureAtlas LoadTextureAtlasFromXml(this ContentManager content, string file)
    {
        var fullPath = Path.Combine(content.RootDirectory, file);

        using var fs = File.OpenRead(fullPath);
        using var reader = XmlReader.Create(fs);

        XDocument doc = XDocument.Load(reader);
        var docRoot = doc.Root;

        var texturePath = docRoot.Element("Texture").Value;
        var texture = content.Load<Texture2D>(texturePath);

        var atlas = new TextureAtlas(texture);

        var regions = (docRoot.Element("Regions")?.Elements("Region") ?? []).ToArray();
        
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

            atlas.AddRegion(name, new TextureRegion(texture, x, y, w, h));
        }

        var animations = (docRoot.Element("Animations")?.Elements("Animation") ?? []).ToArray();
        foreach (var node in animations)
        {
            var name = node.Attribute("name")?.Value;
            var frames = (node.Elements("Frame") ?? []).ToArray();

            if (string.IsNullOrWhiteSpace(name) || frames.Length == 0) continue;

            var frameTime = float.Parse(node.Attribute("frameTime").Value ?? "0");
            var animationFrames = new List<TextureRegion>(frames.Length);
            foreach (var frame in frames)
            {
                var regionName = frame.Attribute("region")?.Value;
                if (string.IsNullOrWhiteSpace(regionName)) continue;

                var region = atlas.GetRegion(regionName);
                animationFrames.Add(region);
            }

            if (animationFrames.Count == 0) continue;

            var animation = new Animation(animationFrames, TimeSpan.FromMilliseconds(frameTime));
            atlas.AddAnimation(name, animation);
        }

        return atlas;
    }
}
