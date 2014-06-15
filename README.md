TMPuzzleXForms
==============

Xamarin.Forms version of TMPuzzle

# About

http://xamar.in/r/XamarinFormsContest #XamarinForms 
のために、Xamarin.Forms に移植したもの。

元ネタは「C#によるiOS、Android、Windowsアプリケーション開発入門」
http://www.amazon.co.jp/dp/4822298345
のサンプルから。

# Technical points

- use Xamarin.Forms 
- XAML ファイルを共有プロジェクトで使う
- ゲームロジックを PCL で共通化
- Azure Moble Services を PCL で共通化

# 注意点

現バージョンでは、XAML ファイルを共通プロジェクトで使うと Xamarin Studio for Mac でビルドができない。Visual Studio 2013 & Xamarin.iOS/Android の場合はビルドができるので、Windows 環境でビルドすること。

