# Copilot Instructions

## Project Guidelines
- User prefers the term "RootRelativePath" for a root-relative path+filename key.
- For the SDB8V6 redesign, preserve all existing WinForms code and forms, add WPF gradually through WinForms/WPF interop, target .NET 8 with C#, and keep existing WinForms forms callable from new WPF windows.
- The `btDisplaySlideShow` button is on `DialogTraverser`, not `DialogDisplay`.
- Diagnose the uneditable `JumpTextBox` in `SlideShowWpf` despite focus/IsReadOnly and guarded text refresh; focus on WinForms-hosted modeless WPF keyboard interop rather than repeating textbox-only changes.
- When showing FileInfoItem width, height, and len in SlideShowWpf, populate zero-valued metadata from the actual loaded image and file rather than only displaying the preexisting finfoList fields, following Main.updateDgvFinfo behavior.
- For `DisplayPreviewSetWpf`, `SlideShowWpf` owns and launches the preview; pass `SlideShowWpf` as host and route all preview callbacks to it, closing the preview when the slideshow closes.