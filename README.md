# TexturePatchwork
Cut out any portion of a texture and paste it onto another texture.

![](Documentation~/example.gif)


# Installation
Add the following address to UnityPackageManager.

```
https://github.com/fuqunaga/TexturePatchwork.git?path=/Packages/ga.fuquna.texturepatchwork
```

# How To Use

### Simply call the method
```
TexturePatchwork.Render()
```

### Use `TexturePatchworkBehaviour`

1. Attach the `TexturePatchworkBehaviour` Component to GameObeject.
2. Set `TexturePatchworkBehaviour.patchParameters`.
3. Get the output texture by `patchParameters.Texture`.


# Reference
UV Texture - https://uvchecker.vinzi.xyz/
