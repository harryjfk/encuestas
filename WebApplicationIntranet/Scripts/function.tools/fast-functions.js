$(document).ready(function () {

	
		$('.mask4').inputmask("9{1,20}.9{0,4}");
		$('.mask4').each(function (i, e) {
			$('.mask4').focusout(function () {
				var val = parseFloat($(e).val());
				$(e).val(val.toFixed(4));
			});
		});
		$.InitTooltip();
	
})