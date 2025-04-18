#!/bin/bash

SESH="my_tmux_session"

tmux has-session -t $SESH 2>/dev/null

if [ $? != 0 ]
then
	tmux new-session -d -s $SESH -n "blank"
	
	tmux new-window -t $SESH -n "git"
	tmux send-keys =t $SESH:git  C-m 'clear' C-m "cd ~/projects/money-manager-v3" C-m 'clear' C-m
	
	tmux new-window -t $SESH -n "backend"
	tmux send-keys -t $SESH:backend "cd ~/projects/money-manager-v3/backend" C-m
	tmux send-keys -t $SESH:backend "docker compose up --build" C-m
	
	tmux new-window -t $SESH -n "frontend"
	tmux send-keys -t $SESH:frontend "cd ~/projects/money-manager-v3/frontend/money-tracker-v3-next" C-m
	tmux send-keys -t $SESH:frontend "docker compose up --build" C-m
	
	tmux new-window -t $SESH -n "psql_db"
	tmux send-keys -t $SESH:psql_db "docker exec -it postgres-master bash" 
	# tmux send-keys -t $SESH:psql_db "psql" C-m # docker db usually hasn't started yet so type in but not enter
	
	tmux select-window -t $SESH:git
fi

tmux attach-session -t $SESH
