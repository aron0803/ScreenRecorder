# ScreenRecorder 螢幕錄影工具

簡單易用的 Windows 螢幕錄影工具，支援全螢幕、區域及視窗錄製，並提供錄音和截圖功能。

## ✨ 功能特色

- 🎥 **多種錄製模式**：全螢幕、自訂區域、指定視窗
- 🎙️ **純音訊錄製**：支援錄音功能 (MP3 格式)
- 📸 **快速截圖**：一鍵截取當前畫面
- ⌨️ **全域熱鍵**：F8 錄音、F9 錄影、F10 截圖
- ⚙️ **可調整設定**：FPS (10-60)、畫質 (低/中/高)
- 💾 **自動分類儲存**：錄影、截圖、錄音自動分類管理
- 📁 **便利管理**：內建檔案清單與快速開啟功能
- 🎹 **按鍵顯示**：可選擇在錄影時顯示按鍵操作

## 💻 系統需求

- **作業系統**：Windows 10 或更新版本 (建議 64 位元)
- **必要元件**：[.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- **硬體建議**：
  - CPU：支援 Intel Quick Sync Video 或 NVIDIA NVENC 的處理器
  - RAM：4 GB 以上
  - 硬碟：足夠的可用空間儲存錄影檔案

## 📥 安裝方式

1. 確認已安裝 **.NET Framework 4.8**
2. 解壓縮下載的 ZIP 檔案到任意資料夾
3. 執行 `ScreenRecorder.exe`

> **注意**：首次執行時，Windows 可能會顯示 SmartScreen 警告，請點選「詳細資訊」→「仍要執行」。

## 🚀 快速開始

### 基本操作

#### 錄影
1. **選擇錄製模式**：
   - 全螢幕錄製
   - 區域錄製（點選「選取範圍」按鈕）

2. **開始錄影**：
   - 點選「開始錄影」按鈕，或按 **F9**
   - 程式視窗會自動最小化

3. **停止錄影**：
   - 再次按 **F9**
   - 錄影檔案自動儲存至 `record/Videos/`

#### 錄音
1. **開始錄音**：按 **F8**
2. **停止錄音**：再次按 **F8**
3. 錄音檔案自動儲存至 `record/Audio/`

#### 截圖
1. **快速截圖**：按 **F10**
2. 截圖檔案自動儲存至 `record/Screenshots/`
3. 截圖不會中斷正在進行的錄影或錄音

### 快捷鍵總覽

| 快捷鍵 | 功能 | 說明 |
|--------|------|------|
| **F8** | 錄音 | 開始/停止純音訊錄製 |
| **F9** | 錄影 | 開始/停止螢幕錄影 |
| **F10** | 截圖 | 快速截取當前畫面 |

### 進階設定

#### FPS（每秒影格數）
- **低品質 (10-20 FPS)**：適合教學示範、簡報錄製
- **標準品質 (30 FPS)**：一般用途，檔案大小適中
- **高品質 (60 FPS)**：遊戲錄製、流暢動畫

#### 畫質
- **低**：適合長時間錄製、檔案大小優先
- **中**：標準品質（推薦）
- **高**：高品質、細節保留

## 📂 檔案結構

```
ScreenRecorder/
├── ScreenRecorder.exe          # 主程式
├── ScreenRecorderLib.dll       # 錄影核心函式庫
├── ScreenRecorder.exe.config   # 程式設定檔
├── config.ini                  # 使用者設定（自動建立）
├── UserManual_ZH-TW.md         # 詳細使用手冊
├── CHANGELOG.md                # 更新日誌
├── MEMORY_MANAGEMENT.md        # 記憶體管理說明
├── TESTING_GUIDE.md            # 測試指南
├── README.md                   # 本檔案
└── record/                     # 錄製檔案資料夾
    ├── Videos/                 # 錄影檔案 (*.mp4)
    ├── Screenshots/            # 截圖檔案 (*.png)
    └── Audio/                  # 錄音檔案 (*.mp3)
```

## 🔧 新功能 (v1.1)

### ✨ 新增功能
- **錄音功能 (F8)**：支援純音訊錄製
- **檔案分類管理**：自動將錄影、截圖、錄音分類儲存
- **開啟資料夾改進**：直接開啟 `record` 資料夾

### 🐛 問題修正
- **按鍵卡住修正**：修正按 Ctrl+Alt+Del 後按鍵狀態卡住的問題
- **記憶體管理**：確認長時間錄製不會導致記憶體持續增長

詳細更新內容請參考 [CHANGELOG.md](CHANGELOG.md)

## ⚠️ 常見問題

### 程式無法啟動
- 確認已安裝 .NET Framework 4.8
- 以系統管理員身分執行

### 熱鍵無效
- 確認沒有其他程式佔用 F8/F9/F10 快捷鍵
- 重新啟動程式

### 按鍵顯示卡住
- 如果按鍵顯示卡住（例如按 Ctrl+Alt+Del 後）
- 點擊程式視窗即可自動清除卡住的按鍵狀態

### 錄影檔案很大
- 降低 FPS 設定（例如：30 → 20）
- 選擇較低的畫質設定
- 使用區域錄製而非全螢幕

### 長時間錄製會不會記憶體爆滿？
- 不會！程式使用串流寫入技術
- 錄製資料會即時寫入磁碟
- 記憶體使用量穩定在 100-200MB
- 詳細說明請參考 [MEMORY_MANAGEMENT.md](MEMORY_MANAGEMENT.md)

### 錄影畫面卡頓
- 提高 FPS 設定
- 提高畫質設定
- 關閉其他佔用 CPU/GPU 的程式

## 📖 詳細文件

- [使用手冊 (UserManual_ZH-TW.md)](UserManual_ZH-TW.md) - 完整使用說明
- [更新日誌 (CHANGELOG.md)](CHANGELOG.md) - 版本更新記錄
- [記憶體管理 (MEMORY_MANAGEMENT.md)](MEMORY_MANAGEMENT.md) - 記憶體使用說明
- [測試指南 (TESTING_GUIDE.md)](TESTING_GUIDE.md) - 功能測試步驟

## 📝 授權聲明

本專案使用 [ScreenRecorderLib](https://github.com/sskodje/ScreenRecorderLib) 函式庫。

## 🔗 相關資源

- [.NET Framework 4.8 下載](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- [ScreenRecorderLib GitHub](https://github.com/sskodje/ScreenRecorderLib)

---

**版本**：1.1  
**最後更新**：2025-12-01
