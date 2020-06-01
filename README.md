# VLCRenderToBitmap
Example usage of VLC in C#/WPF to render video into memory bitmap (and draw this bitmap later to screen). There is a freeze problem when using the scrollbar to seek the media file.

Generates BitmapSource from VLC video, which can be used in WPF. It's not really effective, since the bitmap is copied inside the code. But it works.

Don't forget to reinstall nuget packages using Update-Package -reinstall in the Package Manager Console.
