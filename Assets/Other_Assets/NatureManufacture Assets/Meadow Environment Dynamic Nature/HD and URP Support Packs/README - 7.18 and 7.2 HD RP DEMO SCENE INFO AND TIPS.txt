About scene:
We set all plants and objects manualy as at moment that we made this scene unity HD SRP doesn't support terrain foliage systems. 
There are 2 demo scenes
- converted scene with unity terrain but without grass (we wait for unity grass system at hd rp)
- scene without terrain data but very pretty and small
No grass system affect the performance. That's why number of saved by baching is so huge but...performance at scene should be good anyway. 
We will change this as soon unity will support terrain or something that we could use to buiuld proper scene. (It should be very soon)

BEFORE YOU START:
- you need Unity 2019.3
- you need HD SRP pipline 7.18, if you use higher please import 7.2 rp support pack. It support most higher versions.
Be patient this tech is so fluid... we coudn't fallow every preview version


Step 1 - Setup Shadows and other render setups.

Find Material section at "HDRenderPipelineAsset" and drag and drop our SSS settings diffusion profiles for foliage into Diffusion profile list:
NM_SSSSettings_Skin_Foliage
NM_SSSSettings_Skin_NM Foliage
NM_SSSSettings_Skin_NM Foliage Trees
Without this foliage materials will not become affected by scattering and render will be messed.

Optionaly turn on "increase resolution" at volumetrics at HD Asset file (a bit expensive but I didn't notice big drop so..) 


Step 2 (not necessary) - fill spline system missings: 
Import R.A.M  2019 into project - just spline system folder 

Setp 3 Find HD SRP Demo Small and open it.

Step 4 - HIT PLAY!:)


About scene construction:
- When you enter the forest you are in Forest Render Settings for forest which change expotential fog into volumetric which gives cool forest depth.
  Even at scene view you could enter this area and check how visual aspect of the scene will change. Play with it. 
- There are R.A.M spline objects, with missings until you will import our R.A.M pack, then you could modify it and change  road shape.
- Terrain is based on simple mesh which was vertex painted by 4 layers. Like in book of the dead and other unity hd srp demo scenes.
- Foliage has been planted manualy until unity doesnt support foliage system and terrain on hd srp. We will re-adjust this as soon unity will add
such support.
- Prefab wind manage wind speed and direction at the scene

Play with it give us feedback and lern about hd srp power.

