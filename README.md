# TexturePatchwork
[![npm version](https://badge.fury.io/js/ga.fuquna.texturepatchwork.svg)](https://badge.fury.io/js/ga.fuquna.texturepatchwork)

Cut out any portion of a texture and paste it onto another texture.

https://github.com/fuqunaga/TexturePatchwork/assets/821072/05c79c8d-dcbb-4a06-87ee-68ba7f1d3b8b


# ⬇️ Installation

This package uses the [scoped registry] feature to resolve package
dependencies. 

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html


<br>

**Edit > ProjectSettings... > Package Manager > Scoped Registries**

Enter the following and click the Save button.

```
"name": "fuqunaga",
"url": "https://registry.npmjs.com",
"scopes": [ "ga.fuquna" ]
```

<br>

**Window > Package Manager**

Select `MyRegistries`> `fuqunaga` > `TexturePatchwork` and click the Install button

<br>

# How To Use

### Simply call the method
[`TexturePatchwork.Render()`](https://github.com/fuqunaga/TexturePatchwork/blob/main/Packages/ga.fuquna.texturepatchwork/Scripts/TexturePatchwork.cs#L24)

or

### Use `TexturePatchworkBehaviour`

1. Attach a `TexturePatchworkBehaviour` Component to GameObeject.
2. Set `TexturePatchworkBehaviour.patchParameters`.
3. Get the output texture by `patchParameters.Texture`.


# Reference
UV Texture - https://uvchecker.vinzi.xyz/
