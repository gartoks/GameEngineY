namespace GameEngine.Graphics.RenderSettings {
    public enum BlendFunction {
        Zero,
        One,
        SourceAlpha,
        OneMinusSourceAlpha,
        SourceColor,
        OneMinusSourceColor,
        DestinationAlpha,
        OneMinusDestinationAlpha,
        DestinationColor,
        OneMinusDestinationColor
    }

    public enum BlendMode {
        None,
        Default,
        Replace,
        Additive,
        Overlay,
        Premultiplied,
    }
}
