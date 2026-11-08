# LaTeX 公式编辑器 - 技术方案说明

## 1. 技术选型

### 1.1 开发框架
- **编程语言**: C# 12
- **UI框架**: WPF (Windows Presentation Foundation)
- **运行时**: .NET 8.0

### 1.2 LaTeX渲染方案
- **Web组件**: Microsoft.Web.WebView2
- **渲染库**: KaTeX 0.16.9
- **CDN**: jsDelivr

## 2. 项目结构

```
LaTeX_work/
├── LaTeXFormulaEditor/
│   ├── bin/
│   ├── obj/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── AssemblyInfo.cs
│   ├── LaTeXFormulaEditor.csproj
│   ├── LaTeXRenderer.html
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
└── 技术方案说明.md
```

## 3. 核心技术点

### 3.1 WebView2 集成
- 使用 Microsoft.Web.WebView2 组件嵌入现代 Chromium 浏览器
- 提供高性能的 Web 内容渲染能力
- 支持与 C# 代码的双向通信

### 3.2 KaTeX 渲染
- 使用 KaTeX 进行 LaTeX 公式渲染
- KaTeX 是一个快速、易用的 TeX 渲染库
- 通过 CDN 加载，无需本地安装

### 3.3 架构设计
```
WPF 应用
    ↓
WebView2 控件
    ↓
HTML/JavaScript (KaTeX)
    ↓
LaTeX 渲染
```

## 4. 依赖项

| 依赖项 | 版本 | 说明 |
|--------|------|------|
| Microsoft.NET.Sdk | 8.0 | .NET SDK |
| Microsoft.Web.WebView2 | 1.0.3912.50 | WebView2 组件 |
| KaTeX | 0.16.9 | LaTeX 渲染库 |

## 5. 编译运行

### 5.1 编译
```bash
cd LaTeXFormulaEditor
dotnet build
```

### 5.2 运行
```bash
dotnet run
```

## 6. 功能验证

当前项目已实现以下功能：
- [x] WPF 应用程序框架
- [x] WebView2 集成
- [x] KaTeX LaTeX 渲染
- [x] 基本的用户界面（输入框、渲染按钮）
- [x] 示例公式展示

## 7. 后续规划

1. 增强公式编辑功能
2. 添加公式模板库
3. 实现公式导出功能（图片、PDF）
4. 添加历史记录功能
5. 优化用户体验

## 8. 注意事项

- 需要网络连接以加载 KaTeX CDN 资源
- Windows 系统需安装 WebView2 运行时（Windows 11 已预装）
