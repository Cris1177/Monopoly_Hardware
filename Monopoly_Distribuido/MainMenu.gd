extends Control


func _on_start_pressed() -> void:
	get_tree().change_scene_to_file("res://new-game-project/GameBoard.tscn")


func _on_options_pressed() -> void:
	_show_message("Partida Multijugador", "No hay opciones disponibles todavía.")


func _on_exit_pressed() -> void:
	get_tree().quit()


func _show_message(dialog_title: String, message: String) -> void:
	var dialog := AcceptDialog.new()
	dialog.title = dialog_title
	dialog.dialog_text = message
	add_child(dialog)
	dialog.confirmed.connect(dialog.queue_free)
	dialog.close_requested.connect(dialog.queue_free)
	dialog.popup_centered()