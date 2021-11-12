using Spin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Models;
using Utitshala.Models.LearningDesignElement;
using Utitshala.ViewModels;
using static Utitshala.Services.Interfaces;

namespace Utitshala.Services
{
    /// <summary>
    /// An implementation of the learning design translator class, used to
    /// translate file data to learning design element model data and vice
    /// versa.
    /// </summary>
    public class LearningDesignTranslator : ILearningDesignTranslator
    {
        /// <summary>
        /// Translates a .spd file into learning design element model data.
        /// </summary>
        /// <param name="learningDesignText"></param>
        /// <returns>A list of learning design elements, derived from .spd
        /// file data.</returns>
        public List<LearningDesignElement> TranslateFileToElements(string learningDesignFile)
        {
            // Begin parsing the file
            List<LearningDesignElement> elements = new List<LearningDesignElement>();
            // Parse the file for elements
            string[] split = learningDesignFile.Split(new[] { "\r\n" }, StringSplitOptions.None);
            /* Remove all empty elements and the first two elements (which contain the initialiser metadata), 
             * and a '+' for the iterator below to end successfully */
            split = split.Where(c => c != "")
                .Skip(2).Append("+").ToArray();
            // Parse the file into memory
            // Count helps to delineate the various elements in a sequence, split by use of the '+' character
            int parseCount = 0;
            // In memory holders for the various elements
            LearningDesignElement text = new LearningDesignElement();
            LearningDesignElementSticker sticker = new LearningDesignElementSticker();
            LearningDesignElementImage image = new LearningDesignElementImage();
            LearningDesignElementPresentation presentation = new LearningDesignElementPresentation();
            LearningDesignElementOption option = new LearningDesignElementOption();
            LearningDesignElementExecute execute = new LearningDesignElementExecute();
            LearningDesignElementForum forum = new LearningDesignElementForum();
            LearningDesignElementAudio audio = new LearningDesignElementAudio();
            LearningDesignElementVideo video = new LearningDesignElementVideo();
            // An in memory holder for the strings that will make up that element's data
            List<string> elementHolder = new List<string>();
            // Loop over the string data
            foreach (var ch in split)
            {
                // Check for metadata tag
                if (ch[0] != '#')
                {
                    // '+' symbol is the 'iterator' of these elements in string format
                    // This is excluding the last element
                    if (ch[0] == '+')
                    {
                        // If reached 2 counts of '+', we have reached the end of the element
                        if (parseCount >= 2)
                        {
                            // Switch statement that determines how to interpret the element type
                            if (elementHolder.Where(c => c[0] == '>').Count() != 0)
                            {
                                switch (elementHolder.Where(c => c[0] == '>').First().Split(' ')[1])
                                {
                                    // Case of a pure text element
                                    case "next":
                                        text = new LearningDesignElement();
                                        text.Name = elementHolder[0].Substring(1);
                                        text.TextContent = GetTextContent(elementHolder);
                                        text.NextElement = elementHolder.Where(c => c[0] == '>').Last().Split(' ')[2];
                                        // Add to the sequence of elements
                                        elements.Add(text);
                                        break;
                                    // Case of sticker element
                                    case "sticker":
                                        sticker = new LearningDesignElementSticker();
                                        sticker.LearningDesignElementType = LearningDesignElementType.Sticker;
                                        sticker.Name = elementHolder[0].Substring(1);
                                        sticker.NextElement = elementHolder.Where(c => c[0] == '>').Last().Split(' ')[2];
                                        // Type-specific values
                                        sticker.StickerUrl = elementHolder[3].Split(' ')[2].Replace("\"", "");
                                        sticker.TextContent = GetTextContent(elementHolder);
                                        // Add to the sequence of elements
                                        elements.Add(sticker);
                                        break;
                                    // Case of an image element
                                    case "image":
                                        image = new LearningDesignElementImage();
                                        image.LearningDesignElementType = LearningDesignElementType.Image;
                                        image.Name = elementHolder[0].Substring(1);
                                        image.TextContent = GetTextContent(elementHolder);
                                        image.NextElement = elementHolder.Where(c => c[0] == '>').Last().Split(' ')[2];
                                        // Type-specific values
                                        image.ImageUrl = elementHolder[3].Split(' ')[2].Replace("\"", "");
                                        image.Caption = elementHolder[3].Split(' ')[3].Replace("\"", "");
                                        // Add to the sequence of elements
                                        elements.Add(image);
                                        break;
                                    // Case of a presentation element
                                    case "presentation":
                                        presentation = new LearningDesignElementPresentation();
                                        presentation.LearningDesignElementType = LearningDesignElementType.Presentation;
                                        presentation.Name = elementHolder[0].Substring(1);
                                        presentation.TextContent = GetTextContent(elementHolder);
                                        presentation.NextElement = elementHolder.Where(c => c[0] == '>').Last().Split(' ')[2];
                                        // Type-specific values
                                        presentation.ImageURLs = new string[10]
                                        {
                                            elementHolder[3].Split(' ')[2].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[3].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[4].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[5].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[6].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[7].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[8].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[9].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[10].Replace("\"", ""),
                                            elementHolder[3].Split(' ')[11].Replace("\"", ""),
                                        };
                                        presentation.Compressed = Convert.ToBoolean(elementHolder[3].Split(' ')[12].Replace("\"", ""));
                                        // Add to the sequence of elements
                                        elements.Add(presentation);
                                        break;
                                    // Case of an option element
                                    case "opt":
                                        option = new LearningDesignElementOption();
                                        option.LearningDesignElementType = LearningDesignElementType.Option;
                                        option.Name = elementHolder[0].Substring(1);
                                        // Create the textcontent data
                                        option.TextContent = GetTextContent(elementHolder);
                                        // Type-specific values
                                        option.Options = new List<List<string>>();
                                        foreach (var txt in elementHolder)
                                        {
                                            // If an option call
                                            if (txt[0] == '>')
                                            {
                                                // Add the option to this element
                                                option.Options.Add(new List<string>() { txt.Split(' ')[2], txt.Split(' ')[3].Replace("\"", "") });
                                            }
                                        }
                                        elements.Add(option);
                                        break;
                                    // Case of a close session execution element
                                    case "execute":
                                        execute = new LearningDesignElementExecute();
                                        execute.LearningDesignElementType = LearningDesignElementType.Execute;
                                        execute.Name = elementHolder[0].Substring(1);
                                        // Create the textcontent data
                                        execute.TextContent = GetTextContent(elementHolder);
                                        // Type-specific values
                                        execute.ExecutionName = elementHolder.Where(c => c[0] == '>').First().Split(' ')[2].Replace("\"", "");
                                        elements.Add(execute);
                                        break;
                                    // Case of a forum element
                                    case "forum":
                                        forum = new LearningDesignElementForum();
                                        forum.LearningDesignElementType = LearningDesignElementType.Forum;
                                        forum.Name = elementHolder[0].Substring(1);
                                        forum.TextContent = GetTextContent(elementHolder);
                                        forum.NextElement = elementHolder.Where(c => c[0] == '>').Last().Split(' ')[2];
                                        // Type-specific values
                                        forum.EndDate = Convert.ToDateTime(elementHolder[3].Split(' ')[2].Replace("\"", ""));
                                        forum.WelcomeMessage = elementHolder[3].Split('\"')[3];
                                        break;
                                    default:
                                        Console.WriteLine();
                                        break;
                                }
                            }
                            else
                            {
                                // Is a lesson ending text element
                                text = new LearningDesignElement();
                                text.Name = elementHolder[0].Substring(1);
                                text.TextContent = GetTextContent(elementHolder);
                                // Add to the sequence of elements
                                elements.Add(text);
                                Console.WriteLine();
                            }
                            // Clear the elements
                            parseCount = 0;
                            elementHolder = new List<string>();
                        }
                    }
                    elementHolder.Add(ch);
                    // Catch if the next line is using the element delineator
                    if (ch[0] == '+')
                    {
                        parseCount++;
                    }
                }
            }
            // End
            return elements;
        }

        /// <summary>
        /// Translates a list of learning design element model data into an
        /// .spd file.
        /// </summary>
        /// <param name="model">The learning design and the list of learning design elements.</param>
        /// <returns>A .spd file string.</returns>
        public string TranslateElementsToFile(LearningDesignEditor model)
        {
            // Add the learning design title
            string result = "# " + model.LearningDesign.Name + "\r\n";
            // Add the call to begin on the first element
            result += "> begin " + model.LearningDesignElements.First().Name + "\r\n\r\n";
            // Begin adding the elements
            foreach (var element in model.LearningDesignElements)
            {
                // add the spacing
                switch (element.LearningDesignElementType)
                {
                    case LearningDesignElementType.Text:
                        // Title
                        result += "+ " + element.Name + "\r\n";
                        result += element.TextContent;
                        result += "+\r\n";
                        // Element-specific data
                        result += "> next " + element.NextElement;
                        break;
                    case LearningDesignElementType.Image:
                        // Title
                        result += "+ " + element.Name + "\r\n";
                        result += element.TextContent;
                        result += "+\r\n";
                        // Element-specific data
                        result += "> image " + "\"" 
                            + (element as LearningDesignElementImage).ImageUrl + "\""
                            + " \"" + (element as LearningDesignElementImage).Caption + "\""
                            + "\r\n";
                        result += "> next " + element.NextElement;
                        break;
                    case LearningDesignElementType.Option:
                        // Title
                        result += "+ " + element.Name + "\r\n";
                        result += element.TextContent;
                        result += "+\r\n";
                        // Element-specific data
                        foreach (var opt in (element as LearningDesignElementOption).Options)
                        {
                            result += "> opt " + opt[0] + " \"" + opt[1] + "\"\r\n";
                        }
                        break;
                    case LearningDesignElementType.Sticker:
                        // Title
                        result += "+ " + element.Name + "\r\n";
                        result += element.TextContent;
                        result += "+\r\n";
                        // Element-specific data
                        result += "> sticker " + "\""
                            + (element as LearningDesignElementSticker).StickerUrl + "\""
                            + "\r\n";
                        result += "> next " + element.NextElement;
                        break;
                    default:
                        break;
                }
                result += "\r\n\r\n";
            }
            return result;
        }

        #region Utility Methods
        /// <summary>
        /// Returns a contiguous string of textcontent
        /// from an element list string array.
        /// </summary>
        /// <param name="elements">The element list string array.</param>
        /// <returns>A contiguous string of the textcontent.</returns>
        public string GetTextContent(List<string> elements)
        {
            string textContent = "";
            foreach (var txt in elements.Skip(1))
            {
                if (txt[0] != '+')
                {
                    textContent += txt + "\n";
                }
                else
                {
                    break;
                }
            }
            return textContent;
        }
        #endregion
    }
}