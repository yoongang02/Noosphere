# NooSphere

### 커밋 메세지 규칙</br>
Title: `[#이슈번호] Type: 내용`</br>
Body: 세부 변경사항 설명</br>
| Type | 내용 |
| ------------ | ------------- |
| Feat | 새로운 기능 추가 |
| Fix | 버그 수정 |
| Docs | 문서 수정 |
| Style | 코드 formatting, 세미콜론 누락, 코드 자체의 변경이 없는 경우 |
| Refactor | 코드 리팩토링 |
| Test | 테스트 코드, 리팩토링 테스트 코드 추가 |
| Chore | 패키지 매니저 수정, 그 외 기타 수정 ex) .gitignore |
| Design | CSS 등 사용자 UI 디자인 변경 |
| Comment |	필요한 주석 추가 및 변경 |
| Rename | 파일 또는 폴더 명을 수정하거나 옮기는 작업만인 경우 |
| Remove | 파일을 삭제하는 작업만 수행한 경우 |
| !BREAKING CHANGE | 커다란 API 변경의 경우 |
| !HOTFIX | 급하게 치명적인 버그를 고쳐야 하는 경우 |

***

### 브랜치 규칙</br>

`develop 에서 각자 브랜치를 파서 작업 후, merge 브랜치에 매주 합친뒤 다시 develop 브랜치에 push.`</br>
`최종으로는 develop브랜치를 main에 합쳐서 배포.`

1. **feature 브랜치 컨벤션**: 새로운 기능마다 브랜치를 생성할 때, 해당 기능을 식별할 수 있는 브랜치 네임을 사용. ex) "feature/새로운_기능"
2. **main 브랜치 컨벤션** : 프로젝트가 최종으로 끝나면 main 브랜치에 그동안 작업했던 것들을 push.
3. **merge 브랜치 컨벤션** : 매주 그동안 해왔던 feature 브랜치들을 합쳐서 넣을 예정. ex) “merge/prototype_1”, “merge/prototype_2"
4. **develop 브랜치 컨벤션** : merge 브랜치에 문제가 없다면 develop에 계속 push.
5. **fix 브랜치 컨벤션** : 고칠 기능이 있다면 “fix/고칠기능”과 같은 형태로 브랜치 생성.
