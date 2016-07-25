$( document ).ready(function() {
	ChannelUpdate();
	setTimeout(ChannelUpdate, 10000);
});

function ChannelUpdate() {
	//console.log("Refreshing");
	$.post( "http://127.0.0.1:1234/api/channels")
		.done(function($restData) {
			var $rest = $.parseJSON($restData);
			if ($rest.status == 1) {
				//console.log($rest.data);
				//console.log($rest.data.type.name);
				//console.log($rest.data);
				$('#game').html($rest.data.type.name);
			}
		}
	);
}