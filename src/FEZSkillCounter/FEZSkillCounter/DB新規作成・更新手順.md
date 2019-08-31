# DB新規作成・更新手順

## nugetパッケージのインストール
下記のパッケージをインストールする。

* Microsoft.EntityframeworkCore.Sqlite
* Microsoft.EntityframeworkCore.Tools

## 新規作成時

1. スタートアッププロジェクトをDBを作成するプロジェクトにする
2. 次のコマンドを実行する  
   `Add-Migration InitialCreate`  
3. 次のコマンドでDBファイルを作成する  
    `Update-Database`  

## 更新手順
DBのテーブル構成に変更があった場合には、下記を実施してください。

1. FodyWeavers.xml内のコメントアウトを外す
2. パッケージマネージャから下記のコマンドを実行  
`PM> Add-Migration 変更内容`  
`PM> Update-Database`  
3. FodyWeavers.xml内のコメントアウトを戻す  
