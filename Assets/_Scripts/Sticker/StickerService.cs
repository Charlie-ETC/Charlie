using UnityEngine;

namespace Charlie
{
    class StickerService : Singleton<StickerService>
    {
        Texture AddStickerToTexture(ISticker sticker, Texture texture)
        {
            RenderTexture renderTexture = new RenderTexture(texture.width, texture.height,
                24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            Graphics.Blit(texture, renderTexture);
            Graphics.Blit(sticker.Texture, renderTexture);
            return renderTexture;
        }
    }
}
