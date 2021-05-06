using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace mangadex_sharp_scraper.Classes
{
    public static class Utility
    {
        public static StackPanel GenerateMessageBox(string title, string text,MessageBox box)
        {
            StackPanel panel = new();
            
            // create title block
            TextBlock titleBlock = new();
            titleBlock.Text = title;
            Style titleBlockStyle = new Style();
            titleBlockStyle.Setters.Add(new Setter {Property = TextBlock.FontSizeProperty, Value = 25.0});
            titleBlockStyle.Setters.Add(new Setter {Property = TextBlock.FontWeightProperty, Value = FontWeight.Bold});
            titleBlockStyle.Setters.Add(new Setter {Property = TextBlock.TextAlignmentProperty, Value = TextAlignment.Center});
            titleBlock.Styles.Add(titleBlockStyle);
            
            // text block
            TextBlock textBlock = new();
            textBlock.Text = text;
            Style textBlockStyle = new();
            textBlockStyle.Setters.Add(new Setter {Property = TextBlock.FontSizeProperty, Value = 20.0});
            textBlockStyle.Setters.Add(new Setter {Property = TextBlock.TextAlignmentProperty, Value = TextAlignment.Center});
            textBlockStyle.Setters.Add(new Setter {Property = TextBlock.TextWrappingProperty, Value = TextWrapping.Wrap});
            textBlock.Styles.Add(textBlockStyle);
            
            // close button
            Button close = new Button();
            Style buttonStyle = new();
            buttonStyle.Setters.Add(new Setter {Property = Button.HorizontalAlignmentProperty, Value = HorizontalAlignment.Center});
            buttonStyle.Setters.Add(new Setter {Property = Button.FontSizeProperty, Value = 16.0});
            close.Styles.Add(buttonStyle);
            close.Content = "Close";
            close.Click += (sender, args) =>
            {
                box.Close();
            };
            
            // add to panel
            panel.Children.Add(titleBlock);
            panel.Children.Add(textBlock);
            panel.Children.Add(close);
            return panel;
        }
        
    }
}