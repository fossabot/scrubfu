/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
namespace Scrubfu.Contracts
{
    public interface IParsedScrubfuTag
    {
        string OptionsText { get; set; }

        int? ColumnArrayIndex { get; set; }

        IScrubfuTagOptions GetOptions(string optionsText = null);

        string Apply(string fieldText, IScrubfuTagOptions commandOptions = null);

        bool ValidateTag();
    }
}