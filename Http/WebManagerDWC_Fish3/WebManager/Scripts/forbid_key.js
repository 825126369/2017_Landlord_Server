// 去掉默认回车键影响
document.onkeydown = function () 
{
    if (event.keyCode == 13) 
    {    
		event.returnValue = false;   
	}
}
