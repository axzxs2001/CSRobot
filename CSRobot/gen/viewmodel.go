package viewmodels

import "time"


//${tabledescribe}
type ${tablename}ViewModel struct {

  ${Fields}
  $?{fielddescribe}//${fielddescribe}
  ${fieldname}   $map{dbtype}   `form:"${fieldname}" json:"${fieldname}" uri:"${fieldname}" xml:"${fieldname}" binding:"required"`
  ${Fields}
  
}