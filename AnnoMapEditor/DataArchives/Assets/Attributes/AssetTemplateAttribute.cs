﻿using System;

namespace AnnoMapEditor.DataArchives.Assets.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AssetTemplateAttribute : Attribute
    {
        public string TemplateName { get; init; }


        public AssetTemplateAttribute(string templateName)
        {
            TemplateName = templateName;
        }
    }
}
