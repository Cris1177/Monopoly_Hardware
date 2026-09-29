extends Control

func _on_start_pressed() -> void:
	get_tree().change_scene_to_file("res://Game.tscn")

func _on_options_pressed() -> void:
	print("Opciones del juego aún no implementadas.")

func _on_exit_pressed() -> void:
	get_tree().quit()
