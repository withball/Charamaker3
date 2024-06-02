# Charamaker3

このプロジェクトは以前よりあったゲームエンジンCharamaker2のリメイクプロジェクトだよ！<br>
Charamaker2で作られたゲームはたくさんあり、このへん(https://www.freem.ne.jp/brand/11145)に置いてあります。<br>
未だ作り途中ですが、以下のコンセプトを掲げています。<br>
*完全なコンポーネント指向
ゲームに登場するすべてをWorldで管理し、そのEntityにtextreだとかコンポーネントを追加し、Cameraで見ることで画面に描画される。みたいなフローを貫きます。<br>
<br>
他にもコンセプトがあるような気がしましたが、なかったのでこれで終わりにします。<br>

# ライセンス
MITです。好きに使ってほしいですが、Unityとか言う対抗馬のせいで誰も使ってくれません。

# 実装済みの機能
リソースを使ってCharamakerを起動してみたらだいたいわかると思います。<br>
*画面描画機能
  *図形の描画
  *テクスチャの読み込み及び描画
  *文字の描画
*エンテティ
  *エンテティの枠組み
  *コンポーネントの枠組み
  *複数のエンテティをまとめるCharacterシステム
  *複数のコンポーネントをまとめるMotionシステム
*サウンド
  *効果音の再生
  *BGMの再生
*入力
  *キーボードでの入力(Charamakerでは、WindowFormに引っ張られるので満足に入力はできません)
  *マウスからの入力

# 実装予定の機能
*画面描画機能
  *文字の縁取り
  *エフェクト(たぶんできない)
*エンテティ
  *物理エンジン関連
  *アニメーションを簡単に作れるエンジン
*サウンド
  *エフェクト(たぶんできない)
*入力
  *入力の変換機能
*シーン
  *シーン管理とか
*ファイル管理機能
 *パラメータを読み込むクラスの実装
# Charamaker(ややこしいけど、実行ファイルの方について)
Charamaker2と比べて、複数キャラクターを配置できるようになっています。また、マウスでクリックすることで編集する場所を選べるようになっています。<br>
キャラクターを編集するときは右のテキストボックスの値を変えてF5キーを押します。<br>
モーションもF5キーを押すことで適用します。<br>
既存のリソースのロードはどちらもほっそいテキストボックスに相対パスを入力して行います。motionの方はフォルダ名を入れてエンターを押すと勝手にlsコマンドとかを使ってくれます。<br>
