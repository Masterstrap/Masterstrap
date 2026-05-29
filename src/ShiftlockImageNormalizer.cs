using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Masterstrap.Services
{
    internal static class ShiftlockImageNormalizer
    {
        public const int DefaultTargetSize = 24;

        public static Task NormalizeAsync(string sourcePath, string destPath, int targetSize = DefaultTargetSize, Action<string>? log = null)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                throw new FileNotFoundException("Shiftlock source image not found.", sourcePath);

            targetSize = Math.Clamp(targetSize, 24, 2048);

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                return dispatcher.InvokeAsync(() =>
                {
                    NormalizeCore(sourcePath, destPath, targetSize, log);
                    return Task.CompletedTask;
                }).Task.Unwrap();
            }

            NormalizeCore(sourcePath, destPath, targetSize, log);
            return Task.CompletedTask;
        }

        private static void NormalizeCore(string sourcePath, string destPath, int targetSize, Action<string>? log)
        {
            BitmapFrame frame;
            using (var stream = File.OpenRead(sourcePath))
            {
                var decoder = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);
                if (decoder.Frames.Count == 0)
                    throw new InvalidOperationException("Shiftlock image contains no decodable frames.");

                frame = decoder.Frames[0];
                frame.Freeze();
            }

            if (frame.PixelWidth <= 0 || frame.PixelHeight <= 0)
                throw new InvalidOperationException("Shiftlock image has invalid dimensions.");

            double scale = Math.Min(
                targetSize / (double)frame.PixelWidth,
                targetSize / (double)frame.PixelHeight);

            int drawWidth = Math.Max(1, (int)Math.Round(frame.PixelWidth * scale));
            int drawHeight = Math.Max(1, (int)Math.Round(frame.PixelHeight * scale));
            int offsetX = (targetSize - drawWidth) / 2;
            int offsetY = (targetSize - drawHeight) / 2;

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, targetSize, targetSize));
                dc.DrawImage(frame, new Rect(offsetX, offsetY, drawWidth, drawHeight));
            }

            var bitmap = new RenderTargetBitmap(targetSize, targetSize, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            bitmap.Freeze();

            string? destDir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrWhiteSpace(destDir))
                Directory.CreateDirectory(destDir);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using var output = File.Create(destPath);
            encoder.Save(output);

            log?.Invoke($"[Mods] Shiftlock normalized: {frame.PixelWidth}x{frame.PixelHeight} -> {targetSize}x{targetSize} PNG");
        }
    }
}
