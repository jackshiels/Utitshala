using Spin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Models;
using Utitshala.Models.LearningDesignElement;
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
            // Remove all empty elements and the first two elements (which contain the initialiser metadata)
            split = split.Where(c => c != "").Skip(2).ToArray();
            // Parse the file into memory
            // Count helps to delineate the various elements in a sequence, split by use of the '+' character
            int parseCount = 0;
            // In memory holders for the various elements
            LearningDesignElementText text = new LearningDesignElementText();
            LearningDesignElementSticker sticker = new LearningDesignElementSticker();
            LearningDesignElementAudio audio = new LearningDesignElementAudio();
            LearningDesignElementVideo video = new LearningDesignElementVideo();
            LearningDesignElementOption option = new LearningDesignElementOption();
            // An in memory holder for the strings that will make up that element's data
            List<string> elementHolder = new List<string>();
            // Loop over the string data
            foreach (var ch in split)
            {
                // Check for metadata tag
                if (ch[0] != '#')
                {
                    // '+' symbol is the 'iterator' of these elements in string format
                    if (ch[0] == '+')
                    {
                        // If reached 2 counts of '+', we have reached the end of the element
                        if (parseCount == 2)
                        {
                            // Switch statement that determines how to interpret the element type
                            switch (elementHolder[3].Split(' ')[1])
                            {
                                // Case of a pure text element
                                case "next":
                                    text = new LearningDesignElementText();
                                    text.Name = elementHolder[0].Substring(1);
                                    text.TextContent = elementHolder[1];
                                    // Add to the sequence of elements
                                    elements.Add(text);
                                    break;
                                // Case of sticker element
                                case "sticker":
                                    sticker = new LearningDesignElementSticker();
                                    sticker.Name = elementHolder[0].Substring(1);
                                    sticker.StickerUrl = elementHolder[3].Split(' ')[2];
                                    // Add to the sequence of elements
                                    elements.Add(sticker);
                                    break;
                                default:
                                    Console.WriteLine();
                                    break;
                            }
                            // Clear the elements
                            parseCount = 0;
                            elementHolder = new List<string>();
                        }
                        else
                        {
                            // Else continue iterating until the end of the element
                            parseCount++;
                        }
                    }
                    elementHolder.Add(ch);
                }
            }
            // End
            return elements;
        }

        /// <summary>
        /// Translates a list of learning design element model data into an
        /// .spd file.
        /// </summary>
        /// <param name="elements">The list of learning design elements.</param>
        /// <returns>A .spd file string.</returns>
        public string TranslateElementsToFile(List<LearningDesignElement> elements)
        {
            throw new NotImplementedException();
        }
    }
}