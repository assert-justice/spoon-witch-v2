from __future__ import annotations
import sys, os, shutil, subprocess

class Cli():
    args: list[str]
    script_path: str
    data: dict[str, str]
    _original_cwd: str
    _error: str
    def __init__(self) -> None:
        self.args = list(reversed(sys.argv))
        self.script_path = self.args.pop()
        self._original_cwd = self.cwd()
        self.data = dict()
    def has_args(self) -> bool:
        return len(self.args) > 0
    def _report_if_empty(self):
        if not self.has_args():
            raise RuntimeError('Cli out of arguments!')
    def pop(self) -> str:
        self._report_if_empty()
        return self.args.pop()
    def match(self, options: list[str]):
        return self.args[-1] in options
    def cwd(self) -> str:
        return os.getcwd()
    def reset(self) -> None:
        os.chdir(self._original_cwd)
    def cd(self, dirname: str) -> Cli:
        temp = os.path.join(self.cwd(), dirname)
        if os.path.isdir(temp):
            os.chdir(temp)
        else:
            raise RuntimeError('')
        return self
    def copy(self, src: str, dest: str) -> None:
        src = os.path.normpath(src)
        dest = os.path.normpath(dest)
        shutil.copy(src, dest)
    def is_dir(self, path: str) -> bool:
        return os.path.isdir(os.path.join(self.cwd(), path))
    def exists(self, path: str) -> bool:
        return os.path.exists(path)
    def is_file(self, path: str) -> bool:
        return os.path.isfile(path)
    def mkdir(self, path: str) -> None:
        path = os.path.join(self.cwd(), path)
        os.mkdir(path)
    def makedirs(self, path: str) -> None:
        path = os.path.join(self.cwd(), path)
        os.makedirs(path, exist_ok=True)
    def run(self, args: list[str]):
        res = subprocess.run(args)
        try:
            res.check_returncode()
        except subprocess.CalledProcessError as e:
            print(e)
            exit(res.returncode)
            

def prompt(query: str) -> bool:
    query += ' (y/n)?'
    while True:
        text = input('query').lower()
        if text == 'y':
            return True
        elif text == 'n':
            return False
        else:
            print(f'unexpected input "{text}"')
    