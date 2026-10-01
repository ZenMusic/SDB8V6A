# Copilot Instructions

## Project Guidelines
- User prefers the term "RootRelativePath" for a root-relative path+filename key.
- For the SDB8V6 redesign, preserve all existing WinForms code and forms, add WPF gradually through WinForms/WPF interop, target .NET 8 with C#, and keep existing WinForms forms callable from new WPF windows.
- The `btDisplaySlideShow` button is on `DialogTraverser`, not `DialogDisplay`.
- In `DialogTraverser`, when `cbTraverse` is unchecked, traversal must collect files only from `tbDirectoryPath.Text` and must not enter subfolders.
- In `TraverserBG5`, extension patterns should come directly from `gv.SearchExtensions` rather than `GetExtensionsForArgs(args)`. Additionally, traversal media selection must use `searchExtensionCategory` values such as "images" or "videos", not legacy boolean flags such as `bMovies`.
- Diagnose the uneditable `JumpTextBox` in `SlideShowWpf` despite focus/IsReadOnly and guarded text refresh; focus on WinForms-hosted modeless WPF keyboard interop rather than repeating textbox-only changes.
- When showing FileInfoItem width, height, and len in SlideShowWpf, populate zero-valued metadata from the actual loaded image and file rather than only displaying the preexisting finfoList fields, following Main.updateDgvFinfo behavior.
- For `DisplayPreviewSetWpf`, `SlideShowWpf` owns and launches the preview; pass `SlideShowWpf` as host and route all preview callbacks to it, closing the preview when the slideshow closes.
- For WPF display placement, use the application's Display Information data as authoritative: map display rows by the shown device name and coordinates (for example, DISPLAY2 is X=-1500, Y=0), not by Screen.AllScreens array order or assumed monitor indexes.
- When editing the MainWpf Folder and Catalog, retain its enclosing GroupBox with `Grid.Row="3"` and define inner grid rows; do not replace the GroupBox with a bare Grid, as that overlays other outer-grid content.

## AnnotationsWpf Feature
- Use the term "deck" for any image collection.
- Every annotated deck must be stored in its own unique folder containing that deck's images.
- Annotations should support freehand/highlighted object regions, comments connected visually to selections, cross-references, and search across cards/images.
- AnnotationsWpf annotation geometry must remain aligned across monitor resolutions and window sizes; preserve source image dimensions and source-relative/scaled selection geometry. Display saved annotation region outlines when cards are reopened. Persist annotations to the database eventually, using deck-local serialized storage as an interim format.
- The annotation model class in AnnotationsWpf is named `ImageAnnotation`, replacing `DeckAnnotation`.
- `ImageAnnotation` needs a logical secondary identity composed of `DeckName`, `DeckVersion`, `CardNumber`, and `AnnotationId`.
- `AnnotationDeckDocument` includes `Version` and `Description`.
- Deck metadata must use a `<tarotDeckName>.json` file in the opened deck folder. If missing, prompt for a deck Version while displaying read-only full path, tarotDeckName, and DeckId, then create the file.
- The annotated deck creation prompt should also include an editable Description field; Description is optional and may be null.
- AnnotationsWpf should provide a button that opens the deck-details prompt so users can update the deck Version and optional Description after the JSON file has been created.
- Store annotations separately from deck metadata in `<tarotDeckName>annotations.json` (for example, `R-W-Cannotations.json`), and include an image cropped from the selected/lasso region with each annotation, consistent with the application's earlier behavior. Existing annotations may be deleted and must not be migrated or preserved; use a clean new annotations format in `<tarotDeckName>annotations.json` with a captured lasso-region image for each annotation.
- When showing `DeckInfoFilePath` in the deck-details prompt, it belongs in `contentPanel` with the other read-only deck fields, not in `buttonPanel`, which is reserved for action buttons.
- When displaying annotation metadata in the annotation browser, keep multiline comments as one plain-text field and use shared auto-sized grid rows so labels remain aligned with wrapped values.
- In AnnotationsWpf Deck Details, Type defaults to "unknown" instead of "Tarot"; Type textbox is about 20 characters wide with mutually exclusive Tarot, Collection, Folder checkboxes to its right.

## Annotation Display
- Keep `AnnotationBrowserWpf` unchanged.
- Implement the redesigned spatial annotation viewer as a separate WPF window named `AnnotationDisplayWpf`, opened from `AnnotationsWpf`.