dnvm install 1.0.0-beta7
dnvm alias default 1.0.0-beta7
dnvm use default
dnu restore

cd test/test

dnx test -parallel none
