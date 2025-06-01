using AFamiliarWorld.Bot.Commands.Models;
using AFamiliarWorld.Bot.Familiars;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Drawing.Processing;
namespace AFamiliarWorld.Bot.BattleGenerator;

public class PvPImage
{
    private string _BackgroundImagePath = "Assets/PvP/";
    private const string _FamiliarImagePath = "Assets/Familiars/";
    private const string _StatusConditionImagePath = "Assets/StatusConditions/";
    private List<MemoryStream> _imageStreams = new List<MemoryStream>();
    public PvPImage()
    {
        List <string> imagePaths = new List<string>
        {
            "PvPBackgroundDesert.png",
            "PvPBackgroundGrassland.png",
            "PvPBackgroundLava.png",
            "PvPBackgroundWinter.png",
        };
        this._BackgroundImagePath += imagePaths[new Random().Next(0, imagePaths.Count)];
    }
    
    public async Task<MemoryStream> CompileMemoryStreams()
    {
        if (_imageStreams.Count == 0)
        {
            return null;
        }

        // Load the first image stream as the base image.
        _imageStreams[0].Position = 0;
        var gifImage = Image.Load<Rgba32>(_imageStreams[0]);
        // Set frame delay for the base frame.
        gifImage.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = 300;

        // Loop through the remaining streams and add as frames.
        for (int i = 1; i < _imageStreams.Count; i++)
        {
            _imageStreams[i].Position = 0;
            using var nextImage = Image.Load<Rgba32>(_imageStreams[i]);
            // Set frame delay for each added frame.
            nextImage.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = 300;
            gifImage.Frames.AddFrame(nextImage.Frames.RootFrame);
        }

        // Save the animated GIF to a memory stream.
        var outputStream = new MemoryStream();
        await gifImage.SaveAsGifAsync(outputStream);
        outputStream.Position = 0;
        return outputStream;
    }
    
    public async Task GeneratePvPImage(Familiar left, Familiar right, string battleText)
    {
        // Background
        using var backgroundImage = Image.Load<Rgba32>(_BackgroundImagePath);
        var leftFamiliarImagePath = $"{_FamiliarImagePath}{left.Name.ToLower().Replace(" ", "")}.png";
        var rightFamiliarImagePath = $"{_FamiliarImagePath}{right.Name.ToLower().Replace(" ", "")}.png";
        
        // Familiars
        using var leftFamiliarImage = Image.Load<Rgba32>(leftFamiliarImagePath);
        using var rightFamiliarImage = Image.Load<Rgba32>(rightFamiliarImagePath);
        rightFamiliarImage.Mutate(ctx => ctx.Flip(FlipMode.Horizontal));
        rightFamiliarImage.Metadata.ExifProfile = null;

        backgroundImage.Mutate(ctx =>
        {
            ctx.DrawImage(leftFamiliarImage, new Point(170, 576), 1f);
            ctx.DrawImage(rightFamiliarImage, new Point(1061, 580), 1f);
            
        });

        // HP bars
        int barWidth = 275, barHeight = 33, barOffset = 15;
        int leftBarX = 170;
        int BarY = 556 - barOffset;
        int rightBarX = 1061;
        int leftHealthFillWidth = (int)((left.Health / (float)left.MaxHealth) * barWidth);
        int rightHealthFillWidth = (int)((right.Health / (float)right.MaxHealth) * barWidth);
        var leftBarBackground = new Rectangle(leftBarX, BarY, barWidth, barHeight);
        var leftBarFill = new Rectangle(leftBarX, BarY, leftHealthFillWidth, barHeight);
        var rightBarBackground = new Rectangle(rightBarX, BarY, barWidth, barHeight);
        var rightBarFill = new Rectangle(rightBarX, BarY, rightHealthFillWidth, barHeight);
        
        backgroundImage.Mutate<Rgba32>(ctx =>
        {
            ctx.Fill(Color.Red, leftBarBackground);
            ctx.Fill(Color.LimeGreen, leftBarFill);
            ctx.Fill(Color.Red, rightBarBackground);
            ctx.Fill(Color.LimeGreen, rightBarFill);
        });
        
        // Text
        var battleTextBackground = new Rectangle(57, 115, 1436, 54);
        
        backgroundImage.Mutate<Rgba32>(ctx =>
        {
            ctx.Fill(Color.Black, battleTextBackground);

        });
        
        var font = new Font(SystemFonts.Get("Montserrat Medium"), 30);
        var textOptions = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            WrappingLength = 1400,
            Origin = new PointF(
                battleTextBackground.X + battleTextBackground.Width / 2f,
                (battleTextBackground.Y + battleTextBackground.Height / 2f) - 15)
        };
        
        backgroundImage.Mutate(ctx =>
        {
            ctx.Fill(Color.Black, battleTextBackground);
            ctx.DrawText(textOptions, battleText, Color.White);
        });
        
        // Status Conditions
        foreach (var (statusCondition, index) in (await left.GetStatusConditions()).Select((value, i) => (value, i)))
        {
            if(statusCondition == StatusCondition.None || statusCondition == StatusCondition.Stun) continue;
            
            using var statusImage = Image.Load<Rgba32>($"{_StatusConditionImagePath}/{statusCondition.ToString()}.png");
            statusImage.Mutate(ctx => ctx.Resize(64, 64));
            backgroundImage.Mutate(ctx =>
            {
                ctx.DrawImage(statusImage, new Point(65 + (64 * index), 918), 1f);
            });
        }

        foreach (var (statusCondition, index) in (await right.GetStatusConditions()).Select((value, i) => (value, i)))
        {
            if(statusCondition == StatusCondition.None || statusCondition == StatusCondition.Stun) continue;
            
            using var statusImage = Image.Load<Rgba32>($"{_StatusConditionImagePath}/{statusCondition.ToString()}.png");
            statusImage.Mutate(ctx => ctx.Resize(64, 64));
            backgroundImage.Mutate(ctx =>
            {
                ctx.DrawImage(statusImage, new Point(1000 + (64 * index), 918), 1f);
            });
        }
        
        var encoder = new PngEncoder
        {
            ColorType = PngColorType.RgbWithAlpha,
            BitDepth = PngBitDepth.Bit8
        };

        var stream = new MemoryStream();
        await backgroundImage.SaveAsPngAsync(stream, encoder);
        stream.Position = 0;
        this._imageStreams.Add(stream);
    }
        
}