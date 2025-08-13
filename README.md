---

# ElementParameterImporter2026

 Revit 2026 用の CSV パラメータインポーター アドインです。
「ElementParameterExporter2026」からCSV出力したパラメータを読み込み、Revit モデルに一括で値を反映します。
- デスクトップ上の SelectedElements_Parameters.csv を読み込み、指定したパラメータを更新します。
- 変更ログをまとめて表示します。
- 変更対象のパラメータが読み取り専用の場合や存在しない場合はスキップされます。

---

##  CSV 形式 (例)

ElementId,ParameterName,ParameterValue

123456,Length,12.34

123456,Diameter,50.00

123456,Comments,"配管メインライン"

- 1行目はヘッダーとして認識されます。
- 値の変更がない場合はスキップされます。
- CSV は UTF-8 で保存してください。

---

##  インストール方法

1. このリポジトリをクローンまたはダウンロード  
2. `ElementParameterImporter2026.dll` を Revitの Addins フォルダに配置  
3. 以下のような `.addin` ファイルを作成して読み込む：

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Name>ElementParameterImporter2026</Name>
    <Assembly>C:\test\ElementParameterImporter2026\ElementParameterImporter2026\bin\Debug\ElementParameterImporter2026.dll</Assembly>
    <AddInId>4B763C4B-896B-4DCB-AA57-F201A2DAFC87</AddInId>
    <FullClassName>ElementParameterImporter2026.Command</FullClassName>
    <VendorId>IKST</VendorId>
    <VendorDescription>KengoTanaka</VendorDescription>
  </AddIn>
</RevitAddIns>
```

---

 将来の構想（TODO）

・CSV 保存場所の柔軟化

・パラメータマッピング機能

・型変換の強化

・バックアップ機能

・ログの拡張

・選択範囲の指定

・UI 強化

・複数 CSV の一括インポート

---

 作者

田中 健悟

 BIMエンジニア。Revit APIによるアドイン開発を専門としています。
 
 設備分野の実務経験と多国籍チームのマネジメントを経て、建設業界のDX推進を目指しています。
 
 副業でBIM効率化ツールを開発中。開発依頼やコラボ歓迎です。

 Qiitaにて記事公開。
 https://qiita.com/KengoTanaka-BIM/items/ee06deb9bc65dcfecc22

---

 ライセンス & お問い合わせ

ライセンス：MIT（※自由に使ってOK）

質問・案件相談は Issues または GitHub Profile からどうぞ

---
