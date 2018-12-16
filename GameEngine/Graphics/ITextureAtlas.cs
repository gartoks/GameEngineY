namespace GameEngine.Graphics {
    public interface ITextureAtlas : ITexture {
        ITexture GetSubTexture(string name);
    }
}