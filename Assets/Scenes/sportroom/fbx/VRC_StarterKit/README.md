# VRC_StarterKit (SDK3)
- VRChatワールド作成用スターターキットです(VRCSDK3+UdonSharp版)
- VRC_StarterKit_SDK3.unity がSDK3用のSceneでPrefabはPrefabsSDK3/の下にあります。Prefabs/の下はSDK2用なのでSDK3では使えません。
- This is a starter kit for creating a VRChat world.
- VRCSDK3+UdonSharp version. See VRC_StarterKit_SDK3.unity and PrefabsSDK3/.

- Sample World: https://www.vrchat.com/home/world/wrld_b7384ed8-47a8-4da5-8c88-4d5debf149c9

# Notice
- このパッケージはVCC(VRChat Creator Companion)化済のプロジェクト用です。

# How to use
- VCC(VRChat Creator Companion)のUdonSharp Templateでプロジェクトを作成する。
- PackageManagerからPro Builderを入れる
- USharpVideo をインポート(v1.0.0)
- VRC_StarterKit_SDK3_～.unitypackage をインポート
- Assets/VRC_StarterKit/Scenes/VRC_StarterKit_SDK3.unity を開く
- TMP Importerのウィンドウが開くのでImport TMP Essentialsのボタンを押す
- Scene中のWorldInfo_Advanced/PlayerModsを選択して、InspectorからUdon BehaviorにあるUtilitiesを開くと出てくるCompile All UdonSharp Programsを押す(自動で行なわれるため任意)
- メニューのWindow->Rendering->Lighting Settingsを開いて右下のGenerate Lightingを押す(おそらく任意)
- メニューのVRChat SDK->Show Control Panelを開いてログインする
    - SettingsのFuture Proof Publish をOFFにする
    - BuilerでSetup Layers for VRChatとSetup Collision Matrixのボタンを押す
    - "Video Players do not have automatic resync enabled; …" の警告が出ますがAuto Fixはおそらく不要です。
    - Project SettingsのTags and LayersでLayersのUser Layer 22に PostProcessing を設定する(WorldInfo_Advanced.prefabに設定されているPPSv2を使用する場合に必要)
    - BuilderのBuild & Publish for Windowsを押すとビルドされる。(音が鳴ります)
    - World Name, Descriptionを記入して左下の二つあるチェックボックスの上の方にチェックを入れてUploadボタンを押すとprivateでワールドがアップロードされます。

# 判明した注意点
- PlayerModsの反映は開始時のみのため、動的に歩行速度などを変更する場合はUdon/U#で行なって下さい。
- UdonSharpのv0.19.2より前からの更新の場合 Assets/UdonSharp/Plugins をUdonSharpの更新前に一旦消す必要があるっぽいです。
- Unity2018からの更新は複雑でトラップが多くあるため、VRChat公式のドキュメントを見て慎重に行なって下さい。
- VRCSDK2のワールドをVRCSDK3にする場合、新規にVRCSDK3用のプロジェクトを作成して前のプロジェクトからエクスポートした物をインポートした方が安全です。(SDK2用の設定をクリーニングしないと動かないという噂です)
- Unityのプロジェクトがあるディレクトリ名に日本語が含まれているとUdonSharpのインポート時に延々とエラーが出て完了しなくなるようです。
- Unity2019で新規にプロジェクトをセットアップした場合、PostProcessingレイヤが自動的に作成されないようです。Project SettingsのTags and LayersでLayersのUser Layer 22に PostProcessing を追加して下さい。
- Unity2018->Unity2019の更新時にUdonBehaviourのU#のスクリプトの参照が外れた場合、prefabをreimportすると直る可能性があるようです。ProjectでVRC_StarterKitのフォルダのコンテキストメニューからreimportを試してみてください。(最近のUdonSharpは自動でreimportしてくれるようです)
- VCC(VRChat Creator Companion)への移行は恐らくUnityのバージョンアップと同様に慎重に行なう必要がありそうです。現状VCC移行時にVCC対応済パッケージを入れない場合独自にU#のコンポーネントが生成されるため新しいパッケージのインポート時にprefabとIDが合わずU#に設定した内容がリセットされる危険性がありそうです。

# その他
- VTSWP_MockPanorama.prefab を置いておくとSDK2用の時計がそのまま使えるようになっています。(時計用のVRC_Panorama互換の画像を生成しています)
- With VTSWP_MockPanorama.prefab, You can use any SDK2 (vrchat-time-shaders based) clocks.
- WorldInfo.prefab, WorldInfo_Advanced.prefab は移動をSDK2互換にしています(例: 壁登りができます) Use Legacy Locomotion をOFFにするとSDK3の移動になります。
- Mirror_Advanced_2.02.prefab等の_2.02と付いているprefabはver2.02の見た目の鏡などが入っています。
- このStarterKitではUSharpVideoをとりあえず使用していますが、iwaSync3もあるようなので好みに応じて選択すると良いと思います。 https://hoshinolabs.booth.pm/items/2666275
- 高さ調整椅子がありませんが必要な場合は https://mimyquality.booth.pm/items/2996819 に高さ調節機能が付いているようなので使うと良いと思います。
- USharpVideo 1.0.0 で複数画面表示をする場合は USharpVideo/VideoScreen に付いている USharpVideoPlayer の UseSharedMaterial をONにすると良いようです。
- USharpVideoで、ビルド時に"Video Players do not have automatic resync enabled; …" の警告が出ていますがFixすべきなのかは不明なようです。 https://github.com/MerlinVR/USharpVideo/issues/26

# Required assets
- VCC(VRChat Creator Companion) UdonSharp template.
    - VRChat SDK - Base (tested with 3.1.7)
    - VRChat SDK - Worlds (tested with 3.1.7)
    - VRChat Package Resolver Tool (tested with 0.1.15)
    - VRChat Client Simulator (tested with 1.2.2)
    - UdonSharp (tested with 1.1.1)
- USharpVideo (tested with v1.0.0)
- Pro Builder

# License
- Same as SDK2 version.
- Shaders : MIT License. See each assets for detail.
- Fonts : See each license text. (SIL OFL-1.1)
- The other assets :
  CC0 1.0 Universal (CC0 1.0)
  Public Domain Dedication
  https://creativecommons.org/publicdomain/zero/1.0/

# Change log
- 2022-09-06 VCC
    - Migrated to VCC: VCC化
- 2022-09-06
    - Add Quest prefab: Quest対応用prefabを追加
    - Change fonts to MPLUSRounded1c-Regular.ttf (except PlayerCounter): プレーヤカウンタ以外のフォントを変更(MPLUSRounded1c-Regular.ttf)
        - Mplus1-Regular.ttfでなぜか記号類の表示に問題があったため変更。
- 2022-03-16
    - Support SPS-I: SinglePass Stereo Instanced(SPS-I)対応
    - Enable GPU-I: マテリアルのEnable GPU Instancingを有効化
- 2021-11-25
    - Fix mirror&camera layers for open-beta(2021.4.2 build 1154): open-betaでのUIとカメラのレイヤ変更対応
- 2021-11-21
    - Fix new camera lens reflected in mirror: 鏡に新しいカメラのレンズが映らないように修正。
- 2021-11-17
    - Fix new camera reflected in mirror: 鏡に新しいカメラが映らないように修正。
    - Add 2.02Style: VRC_Starter Kit 2.02 の見た目の鏡などを同梱。
    - Change fonts to Mplus1-Regular.ttf (except PlayerCounter): プレーヤカウンタ以外のフォントを変更(Mplus1-Regular.ttf)
    - Add BGM_withVolume.prefab: 音量調整付きBGMボタンを追加。
    - Add MirrorBar.prefab, VideoBar.prefab: 鏡とビデオプレーヤが出る棒を追加。
    - Rename README_SDK3.md -> README.md: README.mdをSDK3版に変更。ほかSDK2用ファイルをリネーム。
    - Fix PlayerMods.cs audio setting not effect: PlayerModsで指定した音系の設定が反映されていなかったバグを修正。
- 2021-06-15
    - USharpVideoを1.0.0に更新
    - update USharpVideo 0.3.0 -> 1.0.0
- 2021-05-19
    - UnU対応
    - WorldInfo.prefab, WorldInfo_Advanced.prefab: ワールド画像用カメラのデフォルト向きを変更(VRCWorldと同じ方向に変更)
    - Add BGM_Off.prefab: デフォルトOFFのBGMボタンのprefabを追加
    - 変更できそうな変数をSerializeFieldからpublicに変更
    - PlayerCounter.prefab: プレーヤーがワールドから抜けた時に表示が更新されない問題を修正
    - VTSWP_MockPanorama.prefab: 動かしたままにしておく必要のないRenderTexture生成用カメラを自動で停止するように修正
    - Add PlayerCounter.prefab: TextMeshPro版のPlayerCounterを追加
    - EntranceSound.cs: ワールドに自分がjoinした際にワールドに既にいるプレーヤ分のjoin音が鳴らないように修正
    - UnU update
    - WorldInfo.prefab, WorldInfo_Advanced.prefab: change default VRCCam pos.
    - Add BGM_Off.prefab
    - SerializeField -> public for some modifiable fields.
    - PlayerCounter.prefab: Fix player count not changed on OnPlayerLeft.
    - VTSWP_MockPanorama.prefab: disable Camera after few seconds.
    - Add PlayerCounter.prefab (TextMeshPro version of PlayerCounter_Quest.prefab)
    - EntranceSound.cs: don't play entrance sound when initializing world.
- 2021-03-28
    - open-beta udon-network fix.
- 2020-11-29
    - First release in SDK3
    - Based on SDK2 version ver2.03.
