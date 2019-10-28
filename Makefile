COMPILER = mcs
DISTPATH = ./dist/
SRCPATH = ./src/
EXENAME = Program.exe

build: clean prog start

prog:
	$(COMPILER) $(SRCPATH)*.cs -out:$(DISTPATH)$(EXENAME)

start:
	mono $(DISTPATH)$(EXENAME)

clean:
	rm -rf $(DISTPATH)*
