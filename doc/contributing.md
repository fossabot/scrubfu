Contributing to scrubfu
======================

The scrubfu team maintains guidelines for contributing to this repo. A scrubfu team member will be happy to explain why a guideline is defined as it is.

General contribution guidance is included in this document. Additional guidance is defined in the documents linked below.

- [Copyright](copyright.md) describes the licensing practices for the project.
- [Contribution Workflow](contribution-workflow.md) describes the workflow that the team uses for considering and accepting changes.

Up for Grabs
------------

The team marks the most straightforward issues as "up for grabs". This set of issues is the place to start if you are interested in contributing but new to the codebase.

- [scrubfu - "up for grabs"](https://github.com/GrindrodBank/scrubfu/labels/up-for-grabs)

Contribution "Bar"
------------------

Project maintainers will merge changes that improve the product significantly and broadly and that align with the [scrubfu roadmap](roadmap.md). 

Maintainers will not merge changes that have narrowly-defined benefits, due to compatibility risk. Other companies are building products on top of scrubfu, too. We may revert changes if they are found to be breaking.

Contributions must also satisfy the other published guidelines defined in this document.

DOs and DON'Ts
--------------

Please do:

* **DO** follow our [coding style](coding-style.md) (C# code-specific)
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** include tests when adding new features. When fixing bugs, start with
  adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up
  it's often better to create new issue than to side track the discussion.
* **DO** blog and tweet (or whatever) about your contributions, frequently!

Please do not:

* **DON'T** make PRs for style changes. 
* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion so we can agree on a direction before you invest a large amount
  of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to scrubfu, file an issue and start a discussion before proceeding.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.

Commit Messages
---------------

Please format commit messages as follows (based on [A Note About Git Commit Messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)):

```git
Summarize change in 50 characters or less

Provide more detail after the first line. Leave one blank line below the
summary and wrap all lines at 72 characters or less.

If the change fixes an issue, leave another blank line after the final
paragraph and indicate which issue is fixed in the specific format
below.

Fix #42
```

Also do your best to factor commits appropriately, not too large with unrelated things in the same commit, and not too small with the same small change applied N times in N different commits.

File Headers
------------

The following file header is the used for scrubfu. Please use it for new files.

```text
/****************************************************
* Copyright (c) 2019, Grindrod Bank Limited
* License MIT: https://opensource.org/licenses/MIT
****************************************************/
```

Copying Files from Other Projects
---------------------------------

scrubfu uses some files from other projects, typically where a binary distribution does not exist or would be inconvenient.

The following rules must be followed for PRs that include files from another project:

- The license of the file is [permissive](https://en.wikipedia.org/wiki/Permissive_free_software_licence).
- The license of the file is left in-tact.
- The contribution is correctly attributed in the [3rd party notices](THIRD-PARTY-NOTICES.TXT) file in the repository, as needed.

Porting Files from Other Projects
;---------------------------------

There are many good algorithms implemented in other languages that would benefit the scrubfu project. The rules for porting a Java file to C#, for example, are the same as would be used for copying the same file, as described above.

[Clean-room](https://en.wikipedia.org/wiki/Clean_room_design) implementations of existing algorithms that are not permissively licensed will generally not be accepted. If you want to create or nominate such an implementation, please create an issue to discuss the idea.
