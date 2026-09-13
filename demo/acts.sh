# Demo acts and tmux layout for sharpmas, sourced by run.sh and record.sh.
#
# sharpmas is a port, so the interesting claim is sameness: identical commands,
# identical output, a different language underneath. The last act puts rustmas
# and sharpmas side by side and runs the same thing in both.

CAP=0.0; MAIN=0.1; SIDE=0.2
RUSTMAS_DIR="${RUSTMAS_DIR:-$HOME/Developer/rustmas}"

build_layout() {
  tmux kill-session -t "$SESSION" 2>/dev/null || true
  rm -f "$CAPTION_FILE"; : > "$CAPTION_FILE"
  dotnet build -v q --nologo >/dev/null 2>&1

  tmux new-session -d -s "$SESSION" -x "$(tput cols)" -y "$(tput lines)"
  tmux set-option -t "$SESSION" -g status off
  tmux set-option -t "$SESSION" -g pane-border-status top
  tmux set-option -t "$SESSION" -g pane-border-format ' #{pane_title} '

  tmux split-window -v -b -l 3 -t "$SESSION":0.0

  CAP="$SESSION":0.0; MAIN="$SESSION":0.1
  tmux select-pane -t "$CAP"  -T 'sharpmas'
  tmux select-pane -t "$MAIN" -T 'sharpmas'

  tmux send-keys -t "$MAIN" 'clear' Enter
  tmux send-keys -t "$CAP" "source demo/lib.sh; caption_loop" Enter
  sleep 0.6
}

want_side_pane() {
  local have
  have=$(tmux list-panes -t "$SESSION" | wc -l | tr -d ' ')
  if [ "$1" = "yes" ] && [ "$have" -lt 3 ]; then
    tmux split-window -h -l 50% -t "$SESSION":0.1
    SIDE="$SESSION":0.2
  elif [ "$1" = "no" ] && [ "$have" -ge 3 ]; then
    tmux kill-pane -t "$SESSION":0.2 2>/dev/null || true
  fi
  sleep 0.3
}

pin_layout() {
  tmux resize-pane -t "$CAP" -y 2 2>/dev/null || true
  sleep 0.3
}

reset_state() {
  local idx
  for idx in $(tmux list-panes -t "$SESSION" -F '#{pane_index}' | grep -v '^0$'); do
    tmux send-keys -t "$SESSION":0."$idx" C-c; sleep 0.2
    tmux send-keys -t "$SESSION":0."$idx" 'clear' Enter
  done
  : > "$CAPTION_FILE"
  sleep 0.4
}

# The README's alias, because the raw invocation is mostly ceremony on screen.
SM='dotnet run --project src/Sharpmas.Cli --'

act1_run_a_day() {
  say "sharpmas: the same Advent of Code tooling, rebuilt in C#."
  say "Same subcommands, same filters. Let's run 2015 day one."
  run_in "$MAIN" "$SM solve -y 2015 -d 1" 4.0
  say "Both parts and their timings, in the same shape rustmas prints."
  pause 2
  say "That shape is the point. The port was allowed to change how, not what."
  pause 2
}

act2_independent_solver() {
  say "Are the answers right? Ask the independent solver, same as rustmas does."
  run_in "$MAIN" "$SM solve -y 2015 -d 1 --validate" 5.0
  say "Correct on both. A second implementation nobody has to log into, used as a check."
  pause 3
}

act3_sum_types() {
  want_side_pane yes; pin_layout
  say "Rust has enums. C# does not, so the port had to find the nearest honest thing."
  run_in "$SIDE" 'grep -E "abstract record|sealed record|private Answer" src/Sharpmas/Domain/Solution/Answer.cs' 3.5
  say "An abstract record with a private constructor and sealed nested cases."
  say "Private, because only a nested type can reach it. That closes the set from outside."
  pause 3
  say "So a switch over Answer really is exhaustive, which is what the enum bought for free."
  run_in "$MAIN" "$SM solve -y 2015 -d 1" 4.0
  pause 2
}

act4_side_by_side() {
  want_side_pane yes; pin_layout
  tmux select-pane -t "$MAIN" -T 'rustmas (Rust)'
  tmux select-pane -t "$SIDE" -T 'sharpmas (C#)'
  run_in "$MAIN" "cd $RUSTMAS_DIR" 0.6
  say "Two tools. Same commands, same filters, same output format."
  say "Rust on the left, C# on the right. Same puzzle, same day."
  run_in "$MAIN" 'cargo run --release --quiet solve -y 2015 -d 1 --validate' 1.0
  run_in "$SIDE" "$SM solve -y 2015 -d 1 --validate" 5.0
  say "Same answers, same verdicts. Only the timings disagree, and they should."
  pause 3
  say "The translation is the exercise. Neither one is a copy of the other's code."
  pause 3
}

run_act() {
  case "$1" in
    1) want_side_pane no; pin_layout; act1_run_a_day ;;
    2) want_side_pane no; pin_layout; act2_independent_solver ;;
    3) act3_sum_types ;;
    4) act4_side_by_side ;;
    *) echo "unknown act: $1" >&2; return 1 ;;
  esac
}
