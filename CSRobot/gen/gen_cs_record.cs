using System;

namespace ${DataBaseName }
{
    /// <summary>
    /// ${TableDescribe}
    /// </summary>
    public class ${TableName}
    {
        ${ Fields}
        $?{ FieldDescribe}/// <summary>
        $?{ FieldDescribe}/// ${FieldDescribe}
        $?{ FieldDescribe}/// </summary>
        $?{ FieldSize}[BField(Length =${ FieldSize},Name = ""${ FieldName}"")]
        public ${ DBType} ${ FieldName}
        { get; set; }
        ${ Fields}
    }
}
