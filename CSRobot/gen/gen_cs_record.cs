using System;

namespace MyNameSpace
{
    /// <summary>
    /// ${tabledescribe}
    /// </summary>
    public class ${tablename}
    {
        ${Fields}
        $?{fielddescribe}/// <summary>
        $?{fielddescribe}/// ${fielddescribe}
        $?{fielddescribe}/// </summary>
        $?{fieldsize}
        [BField(Length=${fieldsize},Name="${fieldname}")]
        public ${dbtype} ${fieldname}
        { get; set; }
        ${Fields}
    }
}