ps ax | grep Biz.BrightOnion | grep -v grep | awk '{print $1}' | xargs kill

