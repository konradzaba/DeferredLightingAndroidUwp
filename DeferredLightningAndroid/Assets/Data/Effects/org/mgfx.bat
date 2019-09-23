del ClearGBuffer.mgfx 
del CombineFinal.mgfx 
del DirectionalLight.mgfx 
del PointLight.mgfx 
del RenderGBuffer.mgfx 

"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2mgfx.exe" ClearGBuffer.fx ClearGBuffer.mgfx /Debug /Profile:OpenGL
"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2mgfx.exe" CombineFinal.fx CombineFinal.mgfx /Debug /Profile:OpenGL
"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2mgfx.exe" DirectionalLight.fx DirectionalLight.mgfx /Debug /Profile:OpenGL
"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2mgfx.exe" PointLight.fx PointLight.mgfx /Debug /Profile:OpenGL
"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2mgfx.exe" RenderGBuffer.fx RenderGBuffer.mgfx /Debug /Profile:OpenGL

copy ClearGBuffer.mgfx C:\Users\elzab\source\repos\DeferredLightningAndroid\DeferredLightningAndroid\Assets\Data\Effects /Y
copy CombineFinal.mgfx C:\Users\elzab\source\repos\DeferredLightningAndroid\DeferredLightningAndroid\Assets\Data\Effects /Y
copy DirectionalLight.mgfx C:\Users\elzab\source\repos\DeferredLightningAndroid\DeferredLightningAndroid\Assets\Data\Effects /Y
copy PointLight.mgfx C:\Users\elzab\source\repos\DeferredLightningAndroid\DeferredLightningAndroid\Assets\Data\Effects /Y
copy RenderGBuffer.mgfx C:\Users\elzab\source\repos\DeferredLightningAndroid\DeferredLightningAndroid\Assets\Data\Effects /Y

 