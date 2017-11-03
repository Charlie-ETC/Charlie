using UnityEngine;

namespace Charlie.Sticker
{
    public class StickerService : Singleton<StickerService>
    {
        public Texture AddStickerToTexture(ISticker sticker, Texture texture,
            Vector2 scale, Vector2 offset)
        {
            RenderTexture renderTexture = new RenderTexture(texture.width, texture.height,
                24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            RenderTexture.active = renderTexture;
            Graphics.CopyTexture(texture, renderTexture);
            Graphics.Blit(sticker.Texture, renderTexture, scale, offset);

            Texture2D targetTexture = new Texture2D(texture.width, texture.height,
                TextureFormat.ARGB32, false);
            targetTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            targetTexture.Apply();
            return targetTexture;
        }

        public Texture AddStickerToTexture(ISticker sticker, Texture texture)
        {
            return AddStickerToTexture(sticker, texture, Vector2.zero, Vector2.zero);
        }
    }
}
