$(document).ready(function () {

	
		$('.mask4').inputmask("9{1,20}.9{0,4}");
		$('.mask4').each(function (i, e) {
			$('.mask4').focusout(function () {
				var val = parseFloat($(e).val());
				$(e).val(val.toFixed(4));
			});
		});
		$('.mask0').inputmask("9{1,20}");
		$('.maskEnvio').inputmask('integer', { min: 1, max: 28,placeholder:"",rightAlign:0,allowMinus:1 });;
		
		$.InitTooltip();
	
})